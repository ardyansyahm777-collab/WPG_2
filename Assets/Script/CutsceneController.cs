using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Pasang di scene DayCard pada GameObject apapun.
/// Menampilkan teks "DAY X" dengan efek typing mesin ketik,
/// lalu memanggil DayTransitionManager.CutsceneSelesai().
///
/// Suara ketik menggunakan AudioManager.Instance.PlaySFX(AudioManager.Instance.suaraKetik)
/// — tidak perlu AudioSource tambahan.
/// </summary>
public class CutsceneController : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("TextMeshPro untuk teks DAY X")]
    public TextMeshProUGUI txtHari;

    [Header("Typing Audio Settings")]
    [Tooltip("Jarak waktu antar pemunculan huruf")]
    public float jarakAnterHuruf = 0.12f;
    [Range(0.5f, 1.5f)] public float pitchMinimal = 0.9f;
    [Range(0.5f, 1.5f)] public float pitchMaksimal = 1.2f;

    [Tooltip("Jeda diam setelah semua huruf selesai diketik (detik).")]
    public float jedaSetelahSelesai = 1.5f;

    // =============================================
    void Start()
    {
        Debug.Log("<color=lime>[CutsceneController]</color> Start() dipanggil di scene: " +
                  UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        if (txtHari != null)
            txtHari.text = "";

        StartCoroutine(Routine());
    }

    IEnumerator Routine()
    {
        // Tunggu 2 frame agar DayTransitionManager selesai fade in sebelum mulai animasi
        yield return null;
        yield return null;

        string teksLengkap = "DAY " + DayTransitionManager.HariSekarangTransisi;
        Debug.Log($"<color=lime>[CutsceneController]</color> Mulai typing: '{teksLengkap}'");

        yield return StartCoroutine(TypingEffect(teksLengkap));

        yield return new WaitForSecondsRealtime(jedaSetelahSelesai);

        // Panggil CutsceneSelesai agar DayTransitionManager lanjut ke Gameplay
        if (DayTransitionManager.Instance != null)
        {
            Debug.Log("<color=lime>[CutsceneController]</color> Memanggil CutsceneSelesai().");
            DayTransitionManager.Instance.CutsceneSelesai();
        }
        else
        {
            Debug.LogError("[CutsceneController] DayTransitionManager.Instance NULL! " +
                           "Pastikan DayTransitionManager ada di scene MainMenu dengan DontDestroyOnLoad.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
        }
    }

    IEnumerator TypingEffect(string teks)
    {
        if (txtHari == null) yield break;

        txtHari.text = "";

        foreach (char huruf in teks)
        {
            txtHari.text += huruf;

            // Pakai AudioManager — tidak perlu AudioSource di scene DayCard
            if (huruf != ' ' && AudioManager.Instance != null && AudioManager.Instance.suaraKetik != null)
            {
                // Memanggil fungsi baru dengan menyisipkan parameter pitch acak
                AudioManager.Instance.PlaySFXRandomPitch(
                    AudioManager.Instance.suaraKetik, 
                    1f, 
                    pitchMinimal, 
                    pitchMaksimal
                );
            }

            yield return new WaitForSecondsRealtime(jarakAnterHuruf);
        }
    }
}