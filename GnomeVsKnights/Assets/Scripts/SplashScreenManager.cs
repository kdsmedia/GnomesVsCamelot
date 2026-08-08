using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextScene = "MainMenuScene";

    void Start()
    {
        // Initialize AdMob as early as possible (first scene).
        if (RewardedAdManager.Instance != null)
        {
            RewardedAdManager.Instance.Initialize();
        }

        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Ensure settings are correct
        videoPlayer.timeReference = VideoTimeReference.InternalTime; // Fix timestamp issue
        videoPlayer.skipOnDrop = true; // Prevent frame skipping

        // Listen for errors and fix playback issues
        videoPlayer.errorReceived += HandleVideoError;

        // Load the next scene when the video finishes
        videoPlayer.loopPointReached += LoadNextScene;

        // Prepare the video to avoid delays
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += PlayVideo;
    }

    void HandleVideoError(VideoPlayer source, string message)
    {
        Debug.LogWarning($"Video Error: {message}. Restarting Video.");
        source.Stop();
        source.Play();
    }

    void PlayVideo(VideoPlayer source)
    {
        Debug.Log("Video Prepared - Playing Now.");
        source.Play();
    }

    void LoadNextScene(VideoPlayer source)
    {
        Debug.Log("Splash Video Finished. Loading Main Menu.");
        SceneManager.LoadScene(nextScene);
    }
}
