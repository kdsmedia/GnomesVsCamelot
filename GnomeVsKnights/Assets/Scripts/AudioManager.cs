using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("------Audio Source-----")]
    [SerializeField] private AudioSource musicSource;  // ✅ Changed private to serialized field
    [SerializeField] private AudioSource sfxSource;

    [Header("------Audio Clips-----")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSound;

    // Public properties to allow controlled access
    public AudioSource MusicSource => musicSource;
    public AudioSource SFXSource => sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
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
