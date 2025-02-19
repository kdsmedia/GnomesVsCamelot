using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private Button newGameButton;
    private Button loadGameButton;

    private void Start()
    {
        Debug.Log("Main Menu Loaded.");

        // ✅ Find new buttons dynamically in the Main Menu scene
        newGameButton = GameObject.Find("NewGameButton")?.GetComponent<Button>();
        loadGameButton = GameObject.Find("LoadGameButton")?.GetComponent<Button>();

        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveAllListeners(); // Clear old references
            newGameButton.onClick.AddListener(NewGame);
            Debug.Log("New Game Button Listener Added.");
        }
        else
        {
            Debug.LogError("New Game Button not found in scene!");
        }

        if (loadGameButton != null)
        {
            loadGameButton.onClick.RemoveAllListeners();
            loadGameButton.onClick.AddListener(LoadGame);
            Debug.Log("Load Game Button Listener Added.");
        }
        else
        {
            Debug.LogError("Load Game Button not found in scene!");
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
}
