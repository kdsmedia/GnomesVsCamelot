using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("------ Audio Sources -----")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("------ Audio Clips -----")]
    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;
    public AudioClip winnerMusic;
    public AudioClip gameOverMusic;
    public AudioClip buttonClickSound;

    public AudioSource MusicSource => musicSource;
    public AudioSource SFXSource => sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // ✅ Destroy duplicates first
            return;
        }

        Instance = this;  // ✅ Assign the instance AFTER duplicate check
        DontDestroyOnLoad(gameObject);  // ✅ Ensure persistence AFTER ensuring uniqueness
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (musicSource != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            musicSource.volume = savedVolume;

            if (!musicSource.isPlaying)  // ✅ Ensure music starts playing
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

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
