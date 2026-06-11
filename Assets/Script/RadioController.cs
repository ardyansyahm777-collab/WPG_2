using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Tempelkan script ini ke GameObject radio di scene.
/// Pastikan GameObject punya Collider agar OnMouseDown bisa berfungsi.
/// </summary>
public class RadioController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [Tooltip("Sama persis dengan AudioMixer yang dipakai AudioManager & AudioControl.")]
    public AudioMixer audioMixer;

    [Header("Suara Radio")]
    [Tooltip("AudioClip suara siaran radio (loop).")]
    public AudioClip radioClip;

    [Tooltip("AudioSource khusus untuk radio. Bisa dibuat terpisah atau pakai sfxSource AudioManager.")]
    public AudioSource radioSource;

    [Header("Pengaturan Volume")]
    [Tooltip("Volume BGM saat radio menyala (0.0 - 1.0 linear, akan dikonversi ke dB).")]
    [Range(0f, 1f)]
    public float bgmDimVolume = 0.2f;

    [Tooltip("Volume BGM normal saat radio mati.")]
    [Range(0f, 1f)]
    public float bgmNormalVolume = 1f;

    [Tooltip("Durasi fade dalam detik.")]
    public float fadeDuration = 1.0f;

    // ── State ──────────────────────────────────────────────────────────────
    private bool radioIsOn = false;
    private Coroutine fadeCoroutine;

    // Nama parameter di AudioMixer (harus sama persis seperti yang di-Expose)
    private const string PARAM_BGM   = "musicVolume";   // channel Music / BGM
    private const string PARAM_RADIO = "Radio";          // channel Radio (sesuai gambar mixer)

    // ──────────────────────────────────────────────────────────────────────

    private void Start()
    {
        if (radioSource == null)
        {
            // Buat AudioSource lokal kalau belum di-assign
            radioSource = gameObject.AddComponent<AudioSource>();
            radioSource.loop = true;
            radioSource.playOnAwake = false;
        }

        if (radioClip != null)
            radioSource.clip = radioClip;

        // Pastikan channel Radio dimulai di volume 0 (mute) agar tidak terdengar sebelum dinyalakan
        SetMixerVolume(PARAM_RADIO, 0f);
    }

    private void OnMouseDown()
    {
        ToggleRadio();
    }

    /// <summary>
    /// Bisa dipanggil juga dari script lain atau tombol UI jika diperlukan.
    /// </summary>
    public void ToggleRadio()
    {
        radioIsOn = !radioIsOn;

        if (radioIsOn)
            TurnRadioOn();
        else
            TurnRadioOff();
    }

    // ── ON ─────────────────────────────────────────────────────────────────

    private void TurnRadioOn()
    {
        if (!radioSource.isPlaying)
            radioSource.Play();

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(
            bgmTarget:   bgmDimVolume,   // BGM mengecil
            radioTarget: bgmNormalVolume  // Radio penuh
        ));

        Debug.Log("[RadioController] Radio ON.");
    }

    // ── OFF ────────────────────────────────────────────────────────────────

    private void TurnRadioOff()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(
            bgmTarget:   bgmNormalVolume, // BGM kembali normal
            radioTarget: 0f,              // Radio mute
            onComplete:  () => radioSource.Stop()
        ));

        Debug.Log("[RadioController] Radio OFF.");
    }

    // ── COROUTINE FADE ─────────────────────────────────────────────────────

    private IEnumerator FadeRoutine(float bgmTarget, float radioTarget, System.Action onComplete = null)
    {
        float elapsed = 0f;

        // Baca posisi awal dari Mixer (dalam dB) lalu konversi balik ke linear
        float bgmStart   = GetMixerLinear(PARAM_BGM);
        float radioStart = GetMixerLinear(PARAM_RADIO);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            SetMixerVolume(PARAM_BGM,   Mathf.Lerp(bgmStart,   bgmTarget,   t));
            SetMixerVolume(PARAM_RADIO, Mathf.Lerp(radioStart, radioTarget, t));

            yield return null;
        }

        // Pastikan nilai akhir tepat
        SetMixerVolume(PARAM_BGM,   bgmTarget);
        SetMixerVolume(PARAM_RADIO, radioTarget);

        onComplete?.Invoke();
    }

    // ── HELPER ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Set volume Mixer menggunakan rumus dB yang konsisten dengan AudioControl.cs.
    /// </summary>
    private void SetMixerVolume(string param, float linearValue)
    {
        if (audioMixer == null) return;
        float db = Mathf.Log10(Mathf.Max(0.0001f, linearValue)) * 20f;
        audioMixer.SetFloat(param, db);
    }

    /// <summary>
    /// Baca nilai dB dari Mixer dan konversi kembali ke linear (0–1).
    /// </summary>
    private float GetMixerLinear(string param)
    {
        if (audioMixer == null) return 1f;
        if (audioMixer.GetFloat(param, out float db))
            return Mathf.Pow(10f, db / 20f);
        return 1f;
    }
}