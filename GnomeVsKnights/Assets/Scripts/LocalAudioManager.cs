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

        // ✅ Ensure proper background music for GameScene
        if (AudioManager.Instance != null)
        {
            Debug.Log($"🔊 LocalAudioManager: Scene loaded - {SceneManager.GetActiveScene().name}");

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

    // ✅ Play Game Over Music
    public void PlayGameOverMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.MusicSource.Stop(); // Stop current music
            AudioManager.Instance.PlaySceneMusic("GameOver");
            Debug.Log("💀 LocalAudioManager: Playing Game Over music.");
        }
        else
        {
            Debug.LogWarning("⚠️ LocalAudioManager: No AudioManager found to play Game Over music.");
        }
    }

    // ✅ Play Winner Music
    public void PlayWinnerMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.MusicSource.Stop(); //Stop current music
            AudioManager.Instance.PlaySceneMusic("Winner");
            Debug.Log("🏆 LocalAudioManager: Playing Winner music.");
        }
        else
        {
            Debug.LogWarning("⚠️ LocalAudioManager: No AudioManager found to play Winner music.");
        }
    }

    // ✅ Play Button Click Sound Effect
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
