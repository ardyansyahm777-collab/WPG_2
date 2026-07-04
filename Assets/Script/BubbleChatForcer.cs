using UnityEngine;
using System.Collections;
public class BubbleChatForcer : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Durasi bubble chat tampil sebelum otomatis sembunyi. 0 = tidak auto-sembunyi.")]
    public float durasiTampil = 0f;

    [Tooltip("Durasi fade in/out (detik).")]
    public float durasiTransisi = 0.2f;

    private CanvasGroup canvasGroup;
    private Coroutine routineSembunyi;
    private Coroutine routineFade;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Sembunyikan dengan alpha 0, tapi JANGAN SetActive(false)
        // agar StartCoroutine selalu bisa dipanggil
        canvasGroup.alpha = 0f;
        canvasGroup.interactable   = false;
        canvasGroup.blocksRaycasts = false;
    }

    // =============================================
    // PUBLIC API
    // =============================================

    public void Tampilkan()
    {
        if (routineSembunyi != null)
        {
            StopCoroutine(routineSembunyi);
            routineSembunyi = null;
        }

        // Aktifkan dulu SEBELUM StartCoroutine — ini kunci perbaikannya
        gameObject.SetActive(true);
        canvasGroup.interactable   = true;
        canvasGroup.blocksRaycasts = true;

        StartFade(1f);

        if (durasiTampil > 0f)
            routineSembunyi = StartCoroutine(AutoSembunyi());

        Debug.Log($"<color=yellow>[BubbleChatForcer]</color> Tampil: {gameObject.name}");
    }

    public void Sembunyikan()
    {
        if (routineSembunyi != null)
        {
            StopCoroutine(routineSembunyi);
            routineSembunyi = null;
        }

        canvasGroup.interactable   = false;
        canvasGroup.blocksRaycasts = false;

        if (!gameObject.activeInHierarchy)
        {
            // Tidak bisa coroutine — langsung reset alpha
            canvasGroup.alpha = 0f;
            return;
        }

        StartCoroutine(FadeOutSelesai());
    }

    // =============================================
    // PRIVATE
    // =============================================

    void StartFade(float target)
    {
        if (routineFade != null) StopCoroutine(routineFade);
        routineFade = StartCoroutine(FadeTo(target));
    }

    IEnumerator FadeTo(float target)
    {
        float mulai   = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < durasiTransisi)
        {
            elapsed           += Time.deltaTime;
            canvasGroup.alpha  = Mathf.Lerp(mulai, target, elapsed / durasiTransisi);
            yield return null;
        }

        canvasGroup.alpha = target;
        routineFade = null;
    }

    IEnumerator FadeOutSelesai()
    {
        yield return StartCoroutine(FadeTo(0f));
        // Tidak perlu SetActive(false) — alpha 0 sudah cukup menyembunyikannya
        Debug.Log($"<color=yellow>[BubbleChatForcer]</color> Sembunyi: {gameObject.name}");
    }

    IEnumerator AutoSembunyi()
    {
        yield return new WaitForSeconds(durasiTampil);
        StartCoroutine(FadeOutSelesai());
        routineSembunyi = null;
    }
}