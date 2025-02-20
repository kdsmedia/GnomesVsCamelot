using UnityEngine.SceneManagement;
using UnityEngine;

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
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.MusicSource.Stop();  // ✅ No more errors
        }

        SceneManager.LoadScene("GameScene");
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
