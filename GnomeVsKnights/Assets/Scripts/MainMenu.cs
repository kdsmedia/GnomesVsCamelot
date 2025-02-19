using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public GameObject menuPrefab; // Assign in Inspector

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
            Destroy(gameObject);
        }
    }

    public void NewGame()
    {
        Debug.Log("New Game Button Clicked!");
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void LoadGame()
    {
        Debug.Log("Load Game Button Clicked!");
        SceneManager.LoadScene("GameScene"); 
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Button Clicked!");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
