using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button newGameButton;
    public Button loadGameButton;

    private void Start()
    {
        Debug.Log("Main Menu Loaded.");

        // ✅ Reassign buttons dynamically
        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(NewGame);
            Debug.Log("New Game Button Listener Added.");
        }

        if (loadGameButton != null)
        {
            loadGameButton.onClick.RemoveAllListeners();
            loadGameButton.onClick.AddListener(LoadGame);
            Debug.Log("Load Game Button Listener Added.");
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

    public void OpenOptions()
    {
        Debug.Log("Options Menu Opened");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}
