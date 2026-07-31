using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Diperlukan untuk mendeteksi nama scene
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

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
    public static Button_Manager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        SetupResolutionDropdown();
    }

    // =============================================
    // FITUR TOMBOL ESC (KHUSUS GAMEPLAY)
    // =============================================
    private void Update()
    {
        // Mendeteksi apakah pemain menekan tombol Escape (ESC)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Ambil nama scene yang sedang aktif saat ini
            string sceneAktif = SceneManager.GetActiveScene().name;

            // PENTING: Ganti "GamePlay" sesuai dengan nama scene gameplay kamu persis (case-sensitive)
            if (sceneAktif == "GamePlay")
            {
                PauseButton(); // Jalankan fungsi Pause/Resume otomatis
            }
        }
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
        if (resolutions == null || resolutionIndex >= resolutions.Length) return;

        ButtonClick();

        Resolution resolution = resolutions[resolutionIndex];
        
        // 1. Ubah resolusi layar
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // 2. Paksa semua Canvas di Scene untuk re-layout dan update skala secara instan
        StartCoroutine(RebuildCanvasLayouts());
    }

    private IEnumerator RebuildCanvasLayouts()
    {
        // Tunggu 1 frame agar engine Unity selesai menerapkan resolusi baru
        yield return null;

        // Cari semua Canvas aktif di scene (Canvas Utama & Canvas Transisi)
        Canvas[] allCanvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in allCanvases)
        {
            // Re-evaluasi Canvas Scaler
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.enabled = false;
                scaler.enabled = true; // Refresh paksa komponen scaler
            }

            // Rebuild UI RectTransform
            RectTransform rect = canvas.GetComponent<RectTransform>();
            if (rect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }

        Debug.Log("[Button_Manager] Skala Canvas berhasil diperbarui ke resolusi baru.");
    }

    // --- LOGIKA TOMBOL ---

    public void playButton()
{
    ButtonClick();

    // Hentikan musik menu
    if (AudioManager.Instance != null)
        AudioManager.Instance.musicSource.Stop();

    // MainMenu → Tutorial
    if (DayTransitionManager.Instance != null)
    {
        DayTransitionManager.Instance.MulaiDariMainMenu();
    }
    else
    {
        Debug.LogWarning("[Button_Manager] DayTransitionManager tidak ditemukan, load scene langsung.");
        SceneManager.LoadScene("Tutorial");
    }
}

public void goToGameplay()
{
    ButtonClick();

    // Hentikan musik menu
    if (AudioManager.Instance != null)
        AudioManager.Instance.musicSource.Stop();

    // Tutorial → CutScene → Gameplay
    if (DayTransitionManager.Instance != null)
    {
        DayTransitionManager.Instance.MulaiDariTutorial();
    }
    else
    {
        Debug.LogWarning("[Button_Manager] DayTransitionManager tidak ditemukan, load scene langsung.");
        // fallback langsung ke Gameplay
        SceneManager.LoadScene("GamePlay");
    }
}
    public void creditButton()
    {
        ButtonClick();

        // Hentikan musik menu
        if (AudioManager.Instance != null)
            AudioManager.Instance.musicSource.Stop();

        // Tutorial → CutScene → Gameplay
        if (DayTransitionManager.Instance != null)
        {
            DayTransitionManager.Instance.transisiScene("credit");
        }
        else
        {
            Debug.LogWarning("[Button_Manager] DayTransitionManager tidak ditemukan, load scene langsung.");
            // fallback langsung ke Gameplay
            SceneManager.LoadScene("credit");
        }
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

        // Membuka atau menutup panel setting secara otomatis
        if (setting != null)
            setting.SetActive(!setting.activeSelf);
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
        if (AudioManager.Instance != null && AudioManager.Instance.Click != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Click);
        }
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