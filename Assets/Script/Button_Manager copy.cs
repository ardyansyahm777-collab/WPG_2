using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio; // TAMBAHKAN INI UNTUK MEMPERBAIKI EROR SNAPSHOT

public class Button_Manager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject setting;
    public TMP_Dropdown resolutionDropdown;

    [Header("Audio Settings")]
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot pausedSnapshot;

    [Header("Logic")]
    private Resolution[] resolutions;
    public static bool GameIsPaused = false;
    private bool GoMainMenu = false;
    public static Button_Manager Instance;

    private void Awake()
    {
        // Singleton pattern agar mudah diakses
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        SetupResolutionDropdown();
    }

    // --- LOGIKA DROPDOWN RESOLUSI ---
    void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // --- LOGIKA TOMBOL ---

    public void playButton()
    {
        ButtonClick();
        SceneManager.LoadScene("GamePlay");    
    }

    public void settingButton()
    {
        ButtonClick();
        if (setting != null)
            setting.SetActive(!setting.activeSelf);
    }

    public void PauseButton()
    {
        if (GameIsPaused) Resume();
        else Pause();
        
        settingButton(); // Membuka/menutup menu setting saat pause
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        if (normalSnapshot != null) normalSnapshot.TransitionTo(0.5f);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        if (pausedSnapshot != null) pausedSnapshot.TransitionTo(0.01f);
    }

    public void Fullscreen(bool isFullscreen)
    {
        ButtonClick();
        Screen.fullScreen = isFullscreen;
    }

    public void ButtonClick()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.Click);
    }

    public void QuitButton()
    {
        ButtonClick();
        Debug.Log("Game is exiting"); 
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}