using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        Debug.Log("New Game Button Clicked!"); // ✅ Debug log to confirm it's clicking
        Time.timeScale = 1f;  // ✅ Ensure time is reset
        SceneManager.LoadScene("GameScene");  // ✅ Load the game scene
    }

    public void LoadGame()
    {
        Debug.Log("Load Game Button Clicked!"); // ✅ Debug log to confirm it's clicking

        if (PlayerPrefs.HasKey("SavedScene"))
        {
            string savedScene = PlayerPrefs.GetString("SavedScene");
            Debug.Log("Loading saved game scene: " + savedScene);
            Time.timeScale = 1f;  
            SceneManager.LoadScene(savedScene);  
        }
        else
        {
            Debug.Log("No saved game found! Starting new game...");
            NewGame(); // ✅ If no save is found, start a new game
        }
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
