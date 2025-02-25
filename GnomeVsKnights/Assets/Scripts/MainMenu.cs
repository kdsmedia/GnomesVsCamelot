using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public GameObject menuPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (menuPrefab != null)
            {
                Instantiate(menuPrefab, transform);
            }
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate main menu instances
        }
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySceneMusic("MainMenuScene");
        }
    }

    public void NewGame()
    {
        Debug.Log("New Game Button Clicked!");
        Time.timeScale = 1f;  // Ensure game runs normally

        if (AudioManager.Instance != null && AudioManager.Instance.MusicSource != null)
        {
            AudioManager.Instance.MusicSource.Stop();
        }

        // Destroy old GameManager instance before loading a fresh game
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }

        // Load the Game Scene fresh
        SceneManager.LoadScene("GameScene");
    }


    private IEnumerator InitializeGameAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.1f); // ✅ Small delay to allow scene objects to initialize

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }
    }



    public void LoadGame()
    {
        Debug.Log("Load Game Button Clicked!");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.MusicSource.Stop();  // ✅ No more errors
        }

        SceneManager.LoadScene("GameScene");
    }


    public void QuitGame()
    {
        Debug.Log("Quit Game Button Clicked!");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
