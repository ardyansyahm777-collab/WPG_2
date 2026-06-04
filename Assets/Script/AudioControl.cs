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

        // 3. Daftarkan event dinamis agar saat slider digeser via mouse, fungsi Mixer langsung merespon
        RegisterSliderEvents();
    }

    // ====================================================================
    // LOGIKA OTOMATIS MENEMUKAN SLIDER & TEKS BERDASARKAN GAMBAR HIERARCHY
    // ====================================================================
    private void FindAudioComponentsAutomatically()
    {
        // Cari "volume_container" di objek ini atau di parent/anak manapun (Pencarian Global di objek aktif)
        Transform volumeContainer = FindTransformInHierarchy(transform, "volume_container");

        if (volumeContainer == null)
        {
            // Jika tidak ketemu di sekitar script, coba cari di seluruh Scene sebagai opsi terakhir
            GameObject globalContainer = GameObject.Find("volume_container");
            if (globalContainer != null) volumeContainer = globalContainer.transform;
        }

        if (volumeContainer != null)
        {
            // --- DETEKSI MASTER SLIDER ---
            // Sesuai gambar: volume_container -> Master_slider_conta -> MasterSlider
            Transform masterRoot = volumeContainer.Find("Master_slider_container");
            if (masterRoot != null)
            {
                Transform tSlider = masterRoot.Find("MasterSlider");
                if (tSlider != null) masterSlider = tSlider.GetComponent<Slider>();
                
                Transform tText = masterRoot.Find("Volume");
                if (tText != null) masterText = tText.GetComponent<TextMeshProUGUI>();
            }

            // --- DETEKSI MUSIC SLIDER ---
            // Sesuai gambar: volume_container -> Music_slider_conta -> MusicSlider
            Transform musicRoot = volumeContainer.Find("Music_slider_container");
            if (musicRoot != null)
            {
                Transform tSlider = musicRoot.Find("MusicSlider");
                if (tSlider != null) musicSlider = tSlider.GetComponent<Slider>();
                
                Transform tText = musicRoot.Find("Volume");
                if (tText != null) musicText = tText.GetComponent<TextMeshProUGUI>();
            }

            // --- DETEKSI SFX SLIDER ---
            // Sesuai gambar: volume_container -> SFX_slider_conta -> SfxSlider
            Transform sfxRoot = volumeContainer.Find("SFX_slider_container");
            if (sfxRoot != null)
            {
                Transform tSlider = sfxRoot.Find("SfxSlider");
                if (tSlider != null) sfxSlider = tSlider.GetComponent<Slider>();
                
                Transform tText = sfxRoot.Find("Volume");
                if (tText != null) sfxText = tText.GetComponent<TextMeshProUGUI>();
            }
        }

        // Debug log untuk membantu troubleshooting jika ada penamaan yang typo
        if (masterSlider == null || musicSlider == null || sfxSlider == null)
        {
            Debug.LogWarning("[AudioControl] Deteksi otomatis gagal/sebagian tidak ketemu. Pastikan nama GameObject sama persis dengan Hierarchy!");
        }
    }

    // Fungsi rekursif untuk mencari nama objek secara fleksibel meskipun posisinya berada di dalam objek lain
    private Transform FindTransformInHierarchy(Transform root, string targetName)
    {
        if (root.name == targetName) return root;
        foreach (Transform child in root)
        {
            Transform result = FindTransformInHierarchy(child, targetName);
            if (result != null) return result;
        }
        return null;
    }

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
            // Mengubah nilai slider (0.0 - 1.0) menjadi angka (0 - 10)
            float displayValue = value * 10; 
            
            // "F0" artinya angka bulat tanpa desimal (contoh: 7, bukan 7.5)
            textObj.text = displayValue.ToString("F0"); 
        }
    }

    // ====================================================================
    // REKREASI SLIDER CONTROL KE MIXER (DENGAN RUMUS DESIBEL YANG TEPAT)
    // ====================================================================
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("masterVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20); // Mengonversi linear slider ke skala desibel (dB)
        
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