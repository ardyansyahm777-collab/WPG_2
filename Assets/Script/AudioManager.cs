using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip background;
    public AudioClip vnClick;
    public AudioClip MainMenuBGM; 
    public AudioClip Click;
    public AudioClip pop;
    public AudioClip paper;
    public AudioClip suaraKetik;

    [Header("---------- Audio Mixer Reference ----------")]
    [Tooltip("Tarik file Audio Mixer utama kamu ke sini agar volume tersinkronisasi sejak awal scene dimuat.")]
    public UnityEngine.Audio.AudioMixer audioMixer; // TAMBAHAN REKREASI MIXER
    
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            
            // Langsung panggil volume dari memori sejak detik pertama game dinyalakan
            SinkronkanVolumeAwal();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "MainMenu")
        {
            if (MainMenuBGM != null && musicSource != null)
            {
                musicSource.clip = MainMenuBGM;
                musicSource.loop = true;
                musicSource.Play();
                Debug.Log("[AudioManager] Musik Main Menu mulai diputar.");
            }
        }
    }

    // FUNGSI SINKRONISASI INSTAN (Mencegah Audio Muted di Awal Scene Baru)
    private void SinkronkanVolumeAwal()
    {
        if (audioMixer == null) return;

        // Ambil data volume yang tersimpan atau gunakan default 0.75f
        float master = PlayerPrefs.GetFloat("masterVolume", 0.75f);
        float music = PlayerPrefs.GetFloat("musicVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("sfxVolume", 0.75f);

        // Paksa terapkan ke rumus desibel AudioMixer
        audioMixer.SetFloat("masterVolume", Mathf.Log10(Mathf.Max(0.0001f, master)) * 20);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(Mathf.Max(0.0001f, music)) * 20);
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Max(0.0001f, sfx)) * 20);
        
        Debug.Log("[AudioManager] Volume Mixer berhasil disinkronkan di awal game.");
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlaySFXRandomPitch(AudioClip clip, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            // Atur pitch acak di antara rentang yang diberikan
            sfxSource.pitch = Random.Range(minPitch, maxPitch);
            
            // Putar suaranya
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayMusicOnce(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            musicSource.PlayOneShot(clip, volume);
        }
    }
}