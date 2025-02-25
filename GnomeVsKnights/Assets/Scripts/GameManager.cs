using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    public GameObject[] placementPrefabs;
    public GameObject[] fullGnomes;
    public RectTransform[] gnomeUILocations;
    public Tilemap map;
    public Camera cam;
    private bool touchBegan = false;
    private GameObject placementIndicator = null;
    public Dictionary<Vector3Int, GnomeBase> placedGnomes = new Dictionary<Vector3Int, GnomeBase>();
    public List<GameObject> spawnedKnights = new List<GameObject>();
    private int placementType = 0;

    public KnightSpawnStruct[] knightSpawnInformation;
    public GameObject[] knightPrefabs;
    private int knightSpawnIndex = 0;

    public TMP_Text energyText;
    public TMP_Text waveText;
    public GameObject pauseMenu;
    public GameObject winnerPanel;
    public GameObject gameOverPanel;

    private int playerEnergy = 100;
    public int currentWave = 1;
    public int maxWaves = 5;
    private bool gameEnded = false;

    public bool isFastForward = false;

    private float timeElapsed = 0;
    private float lastSpawnTime = 0;
    private List<int> spawnQueue = new List<int>();

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySceneMusic("GameScene");  // ✅ Play game scene music
            }
        }
    }

    public void Inititialize()
    {
        knightSpawnIndex = 0;
        timeElapsed = 0;
        lastSpawnTime = 0;
        currentWave = 1;
        spawnedKnights.Clear();
        UpdateWaveUI();
        pauseMenu.SetActive(false);
        winnerPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (!this.enabled) return;

        if (gameEnded) return;

        timeElapsed += Time.deltaTime;
        UpdateEnergyUI();

        int input = getInput(0);
        if (input == 1)
        {
            placementType = -1;
            for (int i = 0; i < gnomeUILocations.Length; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(gnomeUILocations[i], getInputLocation()))
                {
                    placementType = i;
                    break;
                }
            }
            InitiatePlacement();
        }
        else if (input == 2)
        {
            UpdatePlacementIndicator();
        }
        else if (input == 3)
        {
            PlaceGnome();
        }

        // Check if the player wins
        if (knightSpawnIndex >= knightSpawnInformation.Length)
        {
            if (spawnedKnights.Count == 0)
            {
                ShowWinnerScreen();
            }
        }
        else
        {

            if (knightSpawnInformation[knightSpawnIndex].spawnTime <= timeElapsed)
            {
                if (knightSpawnInformation[knightSpawnIndex].isWave)
                {
                    currentWave++;
                    UpdateWaveUI();
                }
                foreach (int type in knightSpawnInformation[knightSpawnIndex].knights)
                {
                    spawnQueue.Add(type);
                }
                knightSpawnIndex++;
            }
        }
        if (spawnQueue.Count > 0 && timeElapsed - lastSpawnTime >= 0.1f)
        {
            SpawnKnight(knightPrefabs[spawnQueue[0]]);
            spawnQueue.RemoveAt(0);
            lastSpawnTime = timeElapsed;
        }
    }

    public void ToggleFastForward()
    {
        isFastForward = !isFastForward;
        Time.timeScale = isFastForward ? 2f : 1f; // 2x Speed when active
        Debug.Log("Fast Forward: " + (isFastForward ? "ON" : "OFF"));
    }
    public void KnightReachedEnd()
    {
        Debug.Log("A knight reached the base! Game Over.");
        ShowGameOverScreen();
    }

    public void InitiatePlacement()
    {
        if (placementType != -1)
        {
            if (playerEnergy < 25)
            {
                Debug.Log("Not enough energy to place a gnome!");
                return;
            }

            placementIndicator = Instantiate(placementPrefabs[placementType]);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementIndicator != null)
        {
            Vector3Int cellPos = GetCell(GetWorld(getInputLocation()));
            placementIndicator.transform.position = GetWorld(cellPos) + map.cellSize * 0.5f;
        }
    }

    private void PlaceGnome()
    {
        if (placementType != -1)
        {
            if (fullGnomes.Length == 0)
            {
                Debug.LogError("The fullGnomes array is empty. Add gnome prefabs in the Inspector.");
                return;
            }

            Vector3Int at = GetCell(GetWorld(getInputLocation()));

            //Use hardcoded values instead of cell bounds because if someone accidentally places a cell far away it would be a pain to identify the cause and location
            if (at.x < 0 || at.x > 8 || at.y < 0 || at.y > 4)
            {
                Debug.Log("Cannot place gnome: Out of bounds");
            }
            else if (!placedGnomes.ContainsKey(at))
            {
                if (placementType < 0 || placementType >= fullGnomes.Length)
                {
                    Debug.LogError($"Invalid placementType: {placementType}. Array size: {fullGnomes.Length}");
                    return;
                }

                GnomeBase gnomePrefab = fullGnomes[placementType].GetComponent<GnomeBase>();

                if (gnomePrefab == null)
                {
                    Debug.LogError($"❌ Invalid GnomeBase reference for placementType {placementType}");
                    return;
                }

                if (playerEnergy >= gnomePrefab.cost)
                {
                    GameObject gnome = Instantiate(fullGnomes[placementType]);
                    gnome.transform.position = GetWorld(at) + new Vector3(map.cellSize.x * 0.5f, map.cellSize.y * 0.5f, 0);
                    GnomeBase gnomeData = gnome.GetComponent<GnomeBase>();

                    if (gnomeData != null)
                    {
                        gnomeData.Cell = at;
                        placedGnomes.Add(at, gnomeData);
                        playerEnergy -= gnomeData.cost;
                        UpdateEnergyUI();  // ✅ Immediately update UI to reflect energy cost
                    }
                    else
                    {
                        Debug.LogError("❌ GnomeBase component missing on instantiated object!");
                    }

                    Debug.Log($"✅ Placed {gnomePrefab.name} at {at} - Cost: {gnomePrefab.cost} Energy. Remaining: {playerEnergy}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Not enough energy! Needed: {gnomePrefab.cost}, Available: {playerEnergy}");
                }

            }
            else
            {
                Debug.Log("Cannot place gnome: Spot is already occupied.");
            }
            Destroy(placementIndicator);
            placementIndicator = null;
        }
    }

    private void SpawnKnight(GameObject prefab)
    {
        GameObject knight = Instantiate(prefab);

        // Randomize spawn row
        int randomRow = UnityEngine.Random.Range(0, 5);
        Vector3Int spawnCell = new Vector3Int(9, randomRow, 0);
        knight.transform.position = GetWorld(spawnCell) + map.cellSize * 0.5f;

        spawnedKnights.Add(knight);
    }

    private void ShowWinnerScreen()
    {
        gameEnded = true;
        winnerPanel.SetActive(true);
        Time.timeScale = 0;

        // ✅ Play winner music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCustomMusic(AudioManager.Instance.winnerMusic);
        }
    }

    private void ShowGameOverScreen()
    {
        gameEnded = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;

        // ✅ Play game over music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCustomMusic(AudioManager.Instance.gameOverMusic);
        }
    }

    private int getInput(int button)
    {
        int result = 0;
        if (Input.GetMouseButtonDown(button)) result = 1;
        else if (Input.GetMouseButton(button)) result = 2;
        else if (Input.GetMouseButtonUp(button)) result = 3;

        if (result == 0 && Input.touchSupported && Input.touchCount > 0)
        {
            TouchPhase touchPhase = Input.GetTouch(button).phase;
            switch (touchPhase)
            {
                case TouchPhase.Began:
                    result = 1;
                    touchBegan = true;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    result = touchBegan ? 3 : 0;
                    touchBegan = false;
                    break;
                default:
                    result = touchBegan ? 2 : 0;
                    break;
            }
        }
        return result;
    }

    private Vector2 getInputLocation()
    {
        return touchBegan ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
    }

    public Vector3Int GetCell(Vector3 world)
    {
        return map.WorldToCell(world);
    }

    public Vector3 GetWorld(Vector3Int cell)
    {
        return map.CellToWorld(cell);
    }

    public Vector3 GetWorld(Vector3 camera)
    {
        return cam.ScreenToWorldPoint(camera);
    }

    public void KillGnome(Vector3Int at)
    {
        if (placedGnomes.ContainsKey(at))
        {
            Destroy(placedGnomes[at].gameObject);
            placedGnomes.Remove(at);
        }
    }
    public void ResetGame()
    {
        placedGnomes.Clear();  // Clears all placed gnomes
        map = null;  //  Remove Tilemap reference to prevent errors

        // Reset energy to default value
        playerEnergy = 100;
        UpdateEnergyUI();

        // Reset waves to default values
        currentWave = 1;
        knightSpawnIndex = 0;
        spawnedKnights.Clear();
        spawnQueue.Clear();
        timeElapsed = 0;
        lastSpawnTime = 0;
        UpdateWaveUI();

        // Reset knight spawn timer
        StopAllCoroutines(); // Ensure no old knight spawn routines are running

        // Reset game state
        gameEnded = false;
        pauseMenu.SetActive(false);
        winnerPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // Restart scene music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.MusicSource.Stop();
            AudioManager.Instance.MusicSource.Play();
        }
    }


    public void UpdateEnergyUI()
    {
        energyText.text = playerEnergy.ToString();
    }

    public void UpdateWaveUI()
    {
        waveText.text = $"Wave {currentWave}/{maxWaves}";
    }

    public void TogglePauseMenu()
    {
        bool isPaused = !pauseMenu.activeSelf;
        pauseMenu.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0; // Stop all movement and physics
            AudioListener.pause = true; // Pause all audio
            PauseAllEntities(true);
        }
        else
        {
            Time.timeScale = 1; // Resume game
            AudioListener.pause = false;
            PauseAllEntities(false);
        }
    }

    // Stops all moving objects and animations
    private void PauseAllEntities(bool isPaused)
    {
        KnightBase[] knights = FindObjectsByType<KnightBase>(FindObjectsSortMode.None);
        foreach (KnightBase knight in knights)
        {
            knight.enabled = !isPaused;
        }

        GnomeBase[] gnomes = FindObjectsByType<GnomeBase>(FindObjectsSortMode.None);
        foreach (GnomeBase gnome in gnomes)
        {
            Animator animator = gnome.GetComponent<Animator>();
            if (animator != null) animator.enabled = !isPaused;
        }

        AttackBase[] attacks = FindObjectsByType<AttackBase>(FindObjectsSortMode.None);
        foreach (AttackBase attack in attacks)
        {
            attack.enabled = !isPaused;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Ensure the game resumes
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    public void AddEnergy(int amount)
    {
        playerEnergy += amount;
        UpdateEnergyUI();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySceneMusic("MainMenuScene");  // ✅ Switch back to main menu music
        }

        SceneManager.LoadScene("MainMenuScene");
    }

    public void MarkKnightDeath(GameObject obj)
    {
        spawnedKnights.Remove(obj);
    }
}
