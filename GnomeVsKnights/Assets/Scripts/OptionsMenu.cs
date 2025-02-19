using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsPanel;
   
    public Button applyButton;
    public Button restoreDefaultsButton;
    public Button closeButton;
    public TMP_Text saveMessage;
    public AudioSource buttonClickAudioSource;
    public AudioClip buttonClickSound;
    public Image savingImage;
    public RectTransform savingImageTransform;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider soundSlider;

    private const float DEFAULT_VOLUME = 0.5f; // 50% Default Volume

    private void Start()
    {
        optionsPanel.SetActive(false);
        saveMessage.gameObject.SetActive(false);
        savingImage.gameObject.SetActive(false);
        
        restoreDefaultsButton.onClick.AddListener(RestoreDefaults);
        closeButton.onClick.AddListener(() => { PlayButtonSound(); CloseOptions(); });

        LoadSettings();
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", DEFAULT_VOLUME);
        float soundVolume = PlayerPrefs.GetFloat("SoundVolume", DEFAULT_VOLUME);

        musicSlider.value = musicVolume;
        soundSlider.value = soundVolume;
    }

    public void RestoreDefaults()
    {
        PlayButtonSound();

        // Reset sliders to 50%
        musicSlider.value = DEFAULT_VOLUME;
        soundSlider.value = DEFAULT_VOLUME;

        // Apply new settings
        ApplySettings();
    }

    public void ApplySettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.Save();

        Debug.LogFormat("Settings Applied: Music Volume = " + (musicSlider.value * 100) + "%", 
                        "Sound Volume = " + (soundSlider.value * 100) + "%");

        StartCoroutine(ShowSaveMessage());
    }

    private IEnumerator ShowSaveMessage()
    {
        savingImage.gameObject.SetActive(true);
        saveMessage.gameObject.SetActive(true);
        saveMessage.text = "Saving...";
        
        float elapsedTime = 0f;
        while (elapsedTime < 2.5f)
        {
            savingImageTransform.Rotate(0f, 0f, 100f * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        saveMessage.gameObject.SetActive(false);
        savingImage.gameObject.SetActive(false);
    }

    private void PlayButtonSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
        }
    }

    // 🔹 FIXED: This method now exists
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
}
