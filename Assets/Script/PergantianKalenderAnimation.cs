using UnityEngine;
using System.Collections;

/// <summary>
/// Mengelola animasi kalender saat pergantian hari.
/// Dipanggil oleh GameManager.NPCFinishedTurn() → DayTransitionManager (jika ada).
/// </summary>
public class PergantianKalenderAnimation : MonoBehaviour
{
    [Header("Referensi")]
    public Animator kalenderAnimator;
    [Tooltip("Nama trigger animasi 'flip' kalender.")]
    public string triggerGanti = "GantiHari";
    [Tooltip("Durasi animasi kalender sebelum DayTransitionManager jalan (detik).")]
    public float durasiAnimasi = 1.5f;

    // =============================================
    // Dipanggil GameManager.NPCFinishedTurn()
    // =============================================
    public void TransisiHariBerikutnya()
    {
        StartCoroutine(RoutineGantiHari());
    }

    IEnumerator RoutineGantiHari()
    {
        // Putar animasi kalender (opsional)
        if (kalenderAnimator != null)
        {
            kalenderAnimator.SetTrigger(triggerGanti);
            yield return new WaitForSeconds(durasiAnimasi);
        }
        else
        {
            yield return null;
        }

        // Ambil hari berikutnya dari GameManager
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        int hariBerikutnya = gm != null ? gm.currentDay + 1 : 2;

        // Serahkan ke DayTransitionManager
        if (DayTransitionManager.Instance != null)
        {
            DayTransitionManager.Instance.MulaiTransisi(hariBerikutnya);
        }
        else if (gm != null)
        {
            // SOLUSI BUG: Sekarang kita masukkan 'hariBerikutnya' sebagai parameter 
            // agar sesuai dengan fungsi NextDay(int targetHari) yang baru di GameManager
            gm.NextDay(hariBerikutnya);   
        }
    }
}