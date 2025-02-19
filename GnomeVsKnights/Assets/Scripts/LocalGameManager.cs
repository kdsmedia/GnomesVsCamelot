using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
//This class should only function as a relay for GameManager.cs. There should be no logic here besides setting values
//upon start and passing method calls that cannot directly reference the GameManager class
public class LocalGameManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject[] placementPrefabs;
    [SerializeField] private GameObject[] fullGnomes;
    [SerializeField] private RectTransform[] gnomeUILocations;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private KnightSpawnStruct[] knightSpawnInformation;
    [SerializeField] private GameObject[] knightPrefabs;
    [SerializeField] private int maxWaves;

    private void Start()
    {
        GameManager.Instance.cam = cam;
        GameManager.Instance.map = tilemap;
        GameManager.Instance.placementPrefabs = placementPrefabs;
        GameManager.Instance.fullGnomes = fullGnomes;
        GameManager.Instance.gnomeUILocations = gnomeUILocations;
        GameManager.Instance.energyText = energyText;
        GameManager.Instance.waveText = waveText;
        GameManager.Instance.pauseMenu = pauseMenu;
        GameManager.Instance.winnerPanel = winnerPanel;
        GameManager.Instance.gameOverPanel = gameOverPanel;
        GameManager.Instance.knightSpawnInformation = knightSpawnInformation;
        GameManager.Instance.knightPrefabs = knightPrefabs;
        GameManager.Instance.maxWaves = maxWaves;

        GameManager.Instance.Inititialize();
    }
}
