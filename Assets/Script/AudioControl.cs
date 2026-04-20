using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro; // Tambahkan ini untuk akses TextMeshPro

public class AudioControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Text Displays")]
    public TextMeshProUGUI masterText; // Tarik objek angka 10 ke sini
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;

    private void Start()
    {
        float savedMaster = PlayerPrefs.GetFloat("masterVolume", 0.75f);
        float savedMusic = PlayerPrefs.GetFloat("musicVolume", 0.75f);
        float savedSfx = PlayerPrefs.GetFloat("sfxVolume", 0.75f);

        // Update posisi visual Slider
        if (masterSlider != null) masterSlider.value = savedMaster;
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSfx;

        // Terapkan ke Mixer
        SetMasterVolume(savedMaster);
        SetMusicVolume(savedMusic);
        SetSfxVolume(savedSfx);

        UpdateText(masterText, savedMaster);
        UpdateText(musicText, savedMusic);
        UpdateText(sfxText, savedSfx);
    }

    private void UpdateText(TextMeshProUGUI textObj, float value)
    {
        if (textObj != null)
        {
            // Jika slider 0-1 dan ingin tampilan 0-10
            float displayValue = value * 10; 
            textObj.text = displayValue.ToString("F0"); // "F0" berarti tanpa angka desimal
        }
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);
        UpdateText(masterText, volume); // Update teks saat digeser
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
        UpdateText(musicText, volume); // Update teks saat digeser
    }

    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
        UpdateText(sfxText, volume); // Update teks saat digeser
    }
}