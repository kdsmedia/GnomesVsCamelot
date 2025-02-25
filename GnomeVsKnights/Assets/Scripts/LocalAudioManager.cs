using UnityEngine;

public class LocalAudioManager : MonoBehaviour
{
    private void Start()
    {
        // ✅ Ensure AudioManager is available
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("⚠️ LocalAudioManager: No AudioManager found! Creating a new one.");

            GameObject audioManagerGO = new GameObject("AudioManager");
            AudioManager newAudioManager = audioManagerGO.AddComponent<AudioManager>();
        }

        // ✅ Play GameScene music initially
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySceneMusic("GameScene");
            Debug.Log("🎶 LocalAudioManager: Playing GameScene music.");
        }
    }

    // ✅ Play Game Over Music
    public void PlayGameOverMusic()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.MusicSource != null)
        {
            AudioManager.Instance.MusicSource.Stop();  // ✅ Stop current music
            AudioManager.Instance.PlaySceneMusic("GameOver");
            Debug.Log("💀 LocalAudioManager: Playing Game Over music.");
        }
    }

    // ✅ Play Winner Music
    public void PlayWinnerMusic()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.MusicSource != null)
        {
            AudioManager.Instance.MusicSource.Stop();  // ✅ Stop current music
            AudioManager.Instance.PlaySceneMusic("Winner");
            Debug.Log("🏆 LocalAudioManager: Playing Winner music.");
        }
    }

    // ✅ Play Button Click Sound Effect
    public void PlayButtonClickSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
        }
    }
}
