using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DayTransitionManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string tutorialSceneName = "Tutorial"; // Ditambahkan scene tutorial
    public string cutsceneSceneName = "CutScene";
    public string gameplaySceneName = "Gameplay";

    [Header("Fade — Image ini HARUS child dari GameObject ini")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    public static DayTransitionManager Instance { get; private set; }
    public static int HariSekarangTransisi { get; private set; } = 1;

    private bool cutsceneSelesai = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.raycastTarget = false;
        }
    }

    /// <summary>Dipanggil tombol PLAY di MainMenu.</summary>
    public void MulaiDariMainMenu()
    {
        HariSekarangTransisi = 1;
        // Mengubah alur permainan agar masuk ke tutorial terlebih dahulu
        StartCoroutine(RoutineKeTutorial());
    }

    public void MulaiDariTutorial()
    {
        HariSekarangTransisi = 1;
        // Mengubah alur permainan agar masuk ke tutorial terlebih dahulu
        StartCoroutine(RoutineKeGameplay());
    }

    IEnumerator RoutineKeTutorial()
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        // FIX: Load tutorialSceneName, bukan gameplaySceneName
        AsyncOperation load = SceneManager.LoadSceneAsync(tutorialSceneName);
        while (!load.isDone) yield return null;
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
    }
    public void transisiScene(string sceneName)
    {
        StartCoroutine(transisi(sceneName));
    }
    IEnumerator transisi(string sceneName)
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        // FIX: Load tutorialSceneName, bukan gameplaySceneName
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone) yield return null;
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    IEnumerator RoutineKeGameplay()
    {
        // Tutorial → CutScene → Gameplay
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        AsyncOperation loadCut = SceneManager.LoadSceneAsync(cutsceneSceneName);
        while (!loadCut.isDone) yield return null;

        yield return null;
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // Tunggu cutscene selesai (tombol / auto timer)
        cutsceneSelesai = false;
        float elapsed = 0f;
        while (!cutsceneSelesai && elapsed < 15f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        AsyncOperation loadGame = SceneManager.LoadSceneAsync(gameplaySceneName);
        while (!loadGame.isDone) yield return null;

        yield return null;
        yield return null;

        // Mulai hari pertama setelah masuk Gameplay
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
            gm.NextDay(HariSekarangTransisi);

        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    /// <summary>Dipanggil oleh GameManager setelah laporan selesai dibaca.</summary>
    public void MulaiTransisi(int hariBerikutnya)
    {
        HariSekarangTransisi = hariBerikutnya;
        StartCoroutine(RoutineKeCutscene());
    }

    IEnumerator RoutineKeCutscene()
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        AsyncOperation load = SceneManager.LoadSceneAsync(cutsceneSceneName);
        while (!load.isDone) yield return null;

        yield return null;

        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        cutsceneSelesai = false;
        float elapsed = 0f;
        while (!cutsceneSelesai && elapsed < 15f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        AsyncOperation loadGame = SceneManager.LoadSceneAsync(gameplaySceneName);
        while (!loadGame.isDone) yield return null;

        yield return null;
        yield return null;

        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
            gm.NextDay(HariSekarangTransisi);

        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    public void CutsceneSelesai()
    {
        cutsceneSelesai = true;
    }

    IEnumerator Fade(float dari, float ke, float durasi)
    {
        if (fadeImage == null) yield break;

        if (ke > dari) fadeImage.raycastTarget = true;

        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < durasi)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(dari, ke, elapsed / durasi);
            fadeImage.color = c;
            yield return null;
        }
        c.a = ke;
        fadeImage.color = c;

        if (ke < dari) fadeImage.raycastTarget = false;
    }

    public static void MulaiTransisiPertama(int hariAwal) => HariSekarangTransisi = hariAwal;
}