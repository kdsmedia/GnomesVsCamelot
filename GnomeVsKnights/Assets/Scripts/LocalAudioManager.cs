using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalAudioManager : MonoBehaviour
{
    private void Start()
    {
        // ✅ If AudioManager does not exist, create it
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("⚠️ LocalAudioManager: No AudioManager found! Creating a new one.");
            
            GameObject audioManagerGO = new GameObject("AudioManager");
            AudioManager newAudioManager = audioManagerGO.AddComponent<AudioManager>();
        }

        // ✅ Ensure GameScene music plays
        if (AudioManager.Instance != null)
        {
            Debug.Log($"🔊 LocalAudioManager: Scene loaded - {gameObject.scene.name}");
            
            if (SceneManager.GetActiveScene().name == "GameScene")
            {
                AudioManager.Instance.PlaySceneMusic("GameScene");
                Debug.Log("🎶 LocalAudioManager: Playing GameScene music.");
            }
        }
        else
        {
            Debug.LogError("❌ LocalAudioManager: Failed to create AudioManager!");
        }
    }

    public void PlayButtonClickSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
        }
        else
        {
            Debug.LogWarning("⚠️ LocalAudioManager: No AudioManager found to play SFX.");
        }
    }
}
