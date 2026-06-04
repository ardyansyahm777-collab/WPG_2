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
    // Struktur yang diharapkan (sesuai gambar Hierarchy):
    //   volume_container
    //     ├── Master_slider_container
    //     │     ├── outline
    //     │     ├── Volume        ← TextMeshProUGUI penampil angka
    //     │     ├── Text          ← label "Master" (tidak dipakai)
    //     │     └── MasterSlider  ← Slider komponen
    //     ├── Music_slider_container
    //     │     ├── outline
    //     │     ├── Volume
    //     │     ├── Text
    //     │     └── MusicSlider
    //     └── SFX_slider_container
    //           ├── outline
    //           ├── Volume
    //           ├── Text
    //           └── SfxSlider
    // ====================================================================
    private void FindAudioComponentsAutomatically()
    {
        // Cari "volume_container" secara rekursif dari root script ini
        Transform volumeContainer = FindTransformInHierarchy(transform, "volume_container");

        // Fallback: cari secara global di seluruh scene
        if (volumeContainer == null)
        {
            GameObject globalContainer = GameObject.Find("volume_container");
            if (globalContainer != null) volumeContainer = globalContainer.transform;
        }

        if (volumeContainer != null)
        {
            // --- DETEKSI MASTER ---
            Transform masterRoot = volumeContainer.Find("Master_slider_container");
            if (masterRoot != null)
            {
                // Slider — cari langsung by name, lalu fallback GetComponentInChildren
                masterSlider = FindSliderInContainer(masterRoot, "MasterSlider");
                // Text Volume — cari "Volume" child langsung
                masterText   = FindTextInContainer(masterRoot, "Volume");
            }

            // --- DETEKSI MUSIC ---
            Transform musicRoot = volumeContainer.Find("Music_slider_container");
            if (musicRoot != null)
            {
                musicSlider = FindSliderInContainer(musicRoot, "MusicSlider");
                musicText   = FindTextInContainer(musicRoot, "Volume");
            }

            // --- DETEKSI SFX ---
            Transform sfxRoot = volumeContainer.Find("SFX_slider_container");
            if (sfxRoot != null)
            {
                sfxSlider = FindSliderInContainer(sfxRoot, "SfxSlider");
                sfxText   = FindTextInContainer(sfxRoot, "Volume");
            }
        }
        else
        {
            Debug.LogWarning("[AudioControl] 'volume_container' tidak ditemukan di Hierarchy!");
        }

        // Laporan debug per-komponen agar mudah dicek
        if (masterSlider == null) Debug.LogWarning("[AudioControl] MasterSlider tidak ditemukan. Cek nama GameObject di Inspector.");
        if (musicSlider  == null) Debug.LogWarning("[AudioControl] MusicSlider tidak ditemukan. Cek nama GameObject di Inspector.");
        if (sfxSlider    == null) Debug.LogWarning("[AudioControl] SfxSlider tidak ditemukan. Cek nama GameObject di Inspector.");
        if (masterText   == null) Debug.LogWarning("[AudioControl] Master Volume text tidak ditemukan.");
        if (musicText    == null) Debug.LogWarning("[AudioControl] Music Volume text tidak ditemukan.");
        if (sfxText      == null) Debug.LogWarning("[AudioControl] SFX Volume text tidak ditemukan.");
    }

    /// <summary>
    /// Cari Slider di dalam container: pertama by exact name,
    /// lalu rekursif, lalu GetComponentInChildren sebagai fallback terakhir.
    /// </summary>
    private Slider FindSliderInContainer(Transform container, string sliderName)
    {
        // 1. Cari langsung by name (child pertama yang cocok)
        Transform t = FindTransformInHierarchy(container, sliderName);
        if (t != null)
        {
            Slider s = t.GetComponent<Slider>();
            if (s != null) return s;
        }

        // 2. Fallback: ambil Slider pertama yang ditemukan dalam container
        return container.GetComponentInChildren<Slider>();
    }

    /// <summary>
    /// Cari TextMeshProUGUI di dalam container by exact name,
    /// fallback ke child pertama yang punya komponen TMP.
    /// Khusus "Volume" karena ada juga "Text" (label) di container yang sama.
    /// </summary>
    private TextMeshProUGUI FindTextInContainer(Transform container, string textName)
    {
        // 1. Cari langsung child dengan nama tepat
        Transform t = container.Find(textName);
        if (t != null)
        {
            TextMeshProUGUI tmp = t.GetComponent<TextMeshProUGUI>();
            if (tmp != null) return tmp;
        }

        // 2. Rekursif di seluruh hierarki container
        t = FindTransformInHierarchy(container, textName);
        if (t != null)
        {
            TextMeshProUGUI tmp = t.GetComponent<TextMeshProUGUI>();
            if (tmp != null) return tmp;
        }

        // 3. Fallback: ambil TMP pertama (hindari mengambil "Text" label)
        foreach (TextMeshProUGUI tmp in container.GetComponentsInChildren<TextMeshProUGUI>())
        {
            // Prioritaskan objek bernama "Volume" agar tidak salah ambil "Text"
            if (tmp.gameObject.name.ToLower().Contains("volume")) return tmp;
        }
        return null;
    }

    // Fungsi rekursif mencari objek by name di seluruh cabang hierarchy
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