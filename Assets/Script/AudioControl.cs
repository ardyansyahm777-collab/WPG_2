using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioControl : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    
    [Header("Sliders (Auto Found)")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Text Displays (Auto Found)")]
    public TextMeshProUGUI masterText; 
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;

    private void Start()
    {
        // 1. Jalankan fungsi otomatis untuk mendeteksi Slider dan Teks di Hierarchy
        FindAudioComponentsAutomatically();

        // 2. Ambil data volume yang tersimpan di PlayerPrefs
        float savedMaster = PlayerPrefs.GetFloat("masterVolume", 0.75f);
        float savedMusic = PlayerPrefs.GetFloat("musicVolume", 0.75f);
        float savedSfx = PlayerPrefs.GetFloat("sfxVolume", 0.75f);

        // Update posisi visual Slider jika objeknya berhasil ditemukan
        if (masterSlider != null) masterSlider.value = savedMaster;
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSfx;

        // Terapkan nilai ke Mixer
        SetMasterVolume(savedMaster);
        SetMusicVolume(savedMusic);
        SetSfxVolume(savedSfx);

        // Update angka visual teks (0 - 10)
        UpdateText(masterText, savedMaster);
        UpdateText(musicText, savedMusic);
        UpdateText(sfxText, savedSfx);

        // 3. Daftarkan event dinamis agar saat slider digeser via mouse, fungsi Mixer langsung merespon
        RegisterSliderEvents();
    }

    // ====================================================================
    // LOGIKA OTOMATIS MENEMUKAN SLIDER & TEKS BERDASARKAN STRUKTUR HIERARCHY
    // ====================================================================
    private void FindAudioComponentsAutomatically()
    {
        // Mencari container utama tempat penampung slider berada
        Transform volumeContainer = transform.Find("volume_container");
        if (volumeContainer == null)
        {
            // Jika script ditempel langsung di dalam container, jadikan objek ini sebagai root pencarian
            volumeContainer = this.transform;
        }

        // --- PENCARIAN ELEMEN MASTER ---
        Transform masterRoot = volumeContainer.Find("Master_slider_conta");
        if (masterRoot != null)
        {
            masterSlider = masterRoot.GetComponentInChildren<Slider>();
            masterText = masterRoot.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
        }

        // --- PENCARIAN ELEMEN MUSIC ---
        Transform musicRoot = volumeContainer.Find("Music_slider_conta");
        if (musicRoot != null)
        {
            musicSlider = musicRoot.GetComponentInChildren<Slider>();
            musicText = musicRoot.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
        }

        // --- PENCARIAN ELEMEN SFX ---
        Transform sfxRoot = volumeContainer.Find("SFX_slider_conta");
        if (sfxRoot != null)
        {
            sfxSlider = sfxRoot.GetComponentInChildren<Slider>();
            sfxText = sfxRoot.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
        }

        // Debug log untuk membantu troubleshooting jika ada yang terlewat
        if (masterSlider == null || musicSlider == null || sfxSlider == null)
        {
            Debug.LogWarning("[AudioControl] Beberapa komponen Slider gagal ditemukan otomatis. Periksa susunan tata nama di Hierarchy kamu!");
        }
    }

    // Mendaftarkan fungsi secara dinamis tanpa perlu menambahkannya di komponen OnValueChanged Slider satu per satu
    private void RegisterSliderEvents()
    {
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider != null)  musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null)    sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void UpdateText(TextMeshProUGUI textObj, float value)
    {
        if (textObj != null)
        {
            float displayValue = value * 10; 
            textObj.text = displayValue.ToString("F0"); 
        }
    }

    // ====================================================================
    // SLIDER CONTROL TO MIXER
    // ====================================================================
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("masterVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        
        PlayerPrefs.SetFloat("masterVolume", volume);
        UpdateText(masterText, volume); 
    }

    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("musicVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        
        PlayerPrefs.SetFloat("musicVolume", volume);
        UpdateText(musicText, volume); 
    }

    public void SetSfxVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        
        PlayerPrefs.SetFloat("sfxVolume", volume);
        UpdateText(sfxText, volume); 
    }
}