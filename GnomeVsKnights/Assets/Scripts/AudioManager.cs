using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("------Audio Source-----")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("------Audio Clips-----")]
    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;
    public AudioClip winnerMusic;
    public AudioClip gameOverMusic;
    public AudioClip buttonClickSound;

    // ✅ Public getters for Music and SFX sources
    public AudioSource MusicSource => musicSource;
    public AudioSource SFXSource => sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // ✅ Keep AudioManager across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (musicSource != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            musicSource.volume = savedVolume;

            // ✅ Ensure music starts playing
            if (!musicSource.isPlaying)
            {
                PlaySceneMusic(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneMusic(scene.name);
    }

    public void PlaySceneMusic(string sceneName)
    {
        if (musicSource == null) return;

        AudioClip newMusic = null;

        if (sceneName == "MainMenuScene")
            newMusic = mainMenuMusic;
        else if (sceneName == "GameScene")
            newMusic = gameSceneMusic;

        if (musicSource.clip != newMusic)
        {
            musicSource.Stop();
            musicSource.clip = newMusic;
            if (newMusic != null)
                musicSource.Play();
        }
    }

    // ✅ Add missing PlayCustomMusic method
    public void PlayCustomMusic(AudioClip musicClip)
    {
        if (musicSource == null || musicClip == null) return;

        if (musicSource.clip != musicClip)
        {
            musicSource.Stop();
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    // ✅ Add missing PlaySFX method
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
