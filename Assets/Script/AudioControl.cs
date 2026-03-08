using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Wajib untuk mengakses Slider

public class AudioControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        float savedMusic = PlayerPrefs.GetFloat("musicVolume", 0.75f);
        float savedSfx = PlayerPrefs.GetFloat("sfxVolume", 0.75f);

        // Update posisi visual Slider
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSfx;

        // Terapkan ke Mixer
        SetMusicVolume(savedMusic);
        SetSfxVolume(savedSfx);
    }

    public void SetMusicVolume(float volume)
    {
        // Menggunakan Mathf.Log10 agar perubahan suara terasa linear di telinga
        audioMixer.SetFloat("musicVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
}