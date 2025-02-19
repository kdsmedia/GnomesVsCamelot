using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class InGameOptions : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button restartButton;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("Saving UI")]
    [SerializeField] private TMP_Text saveMessage;
    [SerializeField] private Image savingImage;
    [SerializeField] private RectTransform savingImageTransform;

    private bool isSaving = false;

    private void Start()
    {
        LoadSettings();

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        applyButton.onClick.AddListener(() => { PlayButtonSound(); ApplySettings(); });
        closeButton.onClick.AddListener(() => { PlayButtonSound(); CloseInGameOptions(); });
        restartButton.onClick.AddListener(() => { PlayButtonSound(); RestartScene(); });

        // Ensure saving UI is hidden initially
        saveMessage?.gameObject.SetActive(false);
        savingImage?.gameObject.SetActive(false);
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        if (musicAudioSource != null)
            musicAudioSource.volume = volume;

        if (AudioManager.Instance != null)
            AudioManager.Instance.MusicSource.volume = volume;  // ✅ Uses public getter
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxAudioSource != null)
            sfxAudioSource.volume = volume;

        if (AudioManager.Instance != null)
            AudioManager.Instance.SFXSource.volume = volume;  // ✅ Uses public getter
    }


    public void ApplySettings()
    {
        if (isSaving) return;

        isSaving = true;

        if (savingImage != null) savingImage.gameObject.SetActive(true);
        if (saveMessage != null)
        {
            saveMessage.gameObject.SetActive(true);
            saveMessage.text = "Saving...";
        }

        StartCoroutine(RotateSavingIcon());

        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();

        Debug.Log("Settings Applied!");

        StartCoroutine(HideSaveMessage());
    }

    private IEnumerator HideSaveMessage()
    {
        yield return new WaitForSeconds(2.5f);
        saveMessage?.gameObject.SetActive(false);
        savingImage?.gameObject.SetActive(false);
        isSaving = false;
    }

    private IEnumerator RotateSavingIcon()
    {
        while (isSaving)
        {
            if (savingImageTransform != null)
                savingImageTransform.Rotate(0f, 0f, 100f * Time.deltaTime);
            yield return null;
        }
    }

    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("GraphicsQuality", index);
    }

    public void CloseInGameOptions()
    {
        Time.timeScale = 1f;
        ApplySettings();
        gameObject.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PlayButtonSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
        }
    }
}
