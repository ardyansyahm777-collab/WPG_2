using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class RadioController : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Suara Radio")]
    public AudioClip radioClip;
    public AudioSource radioSource;

    [Header("Pengaturan Volume")]
    [Range(0f, 1f)] public float bgmDimVolume = 0.2f;
    [Range(0f, 1f)] public float bgmNormalVolume = 1f;
    public float fadeDuration = 1.0f;

    private bool radioIsOn = false;
    private Coroutine fadeCoroutine;

    // Pastikan PARAM_BGM sama persis dengan yang ada di AudioManager ("musicVolume")
    private const string PARAM_BGM   = "musicVolume";   
    private const string PARAM_RADIO = "radioVolume";          

    // Variabel internal untuk mencatat volume linear terakhir secara akurat
    private float currentBgmLinear = 1f;
    private float currentRadioLinear = 0f;

    private void Start()
    {
        if (radioSource == null)
        {
            radioSource = gameObject.AddComponent<AudioSource>();
            radioSource.loop = true;
            radioSource.playOnAwake = false;
        }

        if (radioClip != null)
            radioSource.clip = radioClip;

        // Mulai dengan volume radio 0 (Mute) dan BGM normal
        currentBgmLinear = bgmNormalVolume;
        currentRadioLinear = 0f;

        SetMixerVolume(PARAM_RADIO, currentRadioLinear);
        SetMixerVolume(PARAM_BGM, currentBgmLinear);
    }

    public void ToggleRadio()
    {
        radioIsOn = !radioIsOn;

        if (radioIsOn)
            TurnRadioOn();
        else
            TurnRadioOff();
    }

    private void TurnRadioOn()
    {
        if (!radioSource.isPlaying)
            radioSource.Play();

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(bgmDimVolume, 1f));

        Debug.Log("[RadioController] Radio ON.");
    }

    private void TurnRadioOff()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(bgmNormalVolume, 0f, () => radioSource.Stop()));

        Debug.Log("[RadioController] Radio OFF.");
    }

    private IEnumerator FadeRoutine(float bgmTarget, float radioTarget, System.Action onComplete = null)
    {
        float elapsed = 0f;

        // Mengambil nilai awal langsung dari variabel tracking lokal (100% Akurat)
        float bgmStart = currentBgmLinear;
        float radioStart = currentRadioLinear;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            currentBgmLinear = Mathf.Lerp(bgmStart, bgmTarget, t);
            currentRadioLinear = Mathf.Lerp(radioStart, radioTarget, t);

            SetMixerVolume(PARAM_BGM, currentBgmLinear);
            SetMixerVolume(PARAM_RADIO, currentRadioLinear);

            yield return null;
        }

        // Pastikan nilai akhir presisi pasca-fade selesai
        currentBgmLinear = bgmTarget;
        currentRadioLinear = radioTarget;

        SetMixerVolume(PARAM_BGM, bgmTarget);
        SetMixerVolume(PARAM_RADIO, radioTarget);

        onComplete?.Invoke();
    }

    private void SetMixerVolume(string param, float linearValue)
    {
        if (audioMixer == null) return;
        // Rumus logaritma dB yang aman dari angka 0 murni
        float db = Mathf.Log10(Mathf.Max(0.0001f, linearValue)) * 20f;
        audioMixer.SetFloat(param, db);
    }
}