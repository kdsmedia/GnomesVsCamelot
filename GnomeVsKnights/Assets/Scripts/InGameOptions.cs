using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class InGameOptions : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;

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
        pauseButton.onClick.AddListener(() => { PlayButtonSound(); PauseGame(); });
        resumeButton.onClick.AddListener(() => { PlayButtonSound(); ResumeGame(); });

        // Ensure pause menu & saving UI are hidden initially
        pauseMenu.SetActive(false);
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
            AudioManager.Instance.MusicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxAudioSource != null)
            sfxAudioSource.volume = volume;

        if (AudioManager.Instance != null)
            AudioManager.Instance.SFXSource.volume = volume;
    }

    public void ApplySettings()
    {
        if (isSaving) return; // Prevent multiple saves at once

        isSaving = true;

        // Show saving UI
        savingImage.gameObject.SetActive(true);
        saveMessage.gameObject.SetActive(true);
        saveMessage.text = "Saving...";

        StartCoroutine(RotateSavingIcon());

        // Save settings
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();

        Debug.Log("Settings Applied!");

        StartCoroutine(HideSaveMessage());
    }

    private IEnumerator HideSaveMessage()
    {
        yield return new WaitForSeconds(2.5f); // Wait before hiding

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

    public void PauseGame()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        pauseMenu.SetActive(false);

        // ✅ Hide saving UI when resuming
        saveMessage?.gameObject.SetActive(false);
        savingImage?.gameObject.SetActive(false);
        isSaving = false;
    }

    public void CloseInGameOptions()
    {
        Time.timeScale = 1f;
        ApplySettings();
        gameObject.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;  // ✅ Ensure time is resumed
        AudioListener.pause = false;  // ✅ Ensure audio resumes
        SceneManager.LoadScene("MainMenuScene");
    }



    public void RestartScene()
    {
        Time.timeScale = 1f;  // ✅ Reset time so everything starts fresh
        AudioListener.pause = false;  // ✅ Resume audio

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();  // ✅ Reset all objects before restarting
        }

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
