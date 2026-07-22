using UnityEngine;

public class GameplayTestInitializer : MonoBehaviour
{
    void Awake()
    {
        // 1. Jika game dimulai dari MainMenu, hancurkan objek testing ini agar tidak dobel
        if (DayTransitionManager.Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("<color=yellow>[Testing]</color> Kamu melakukan PLAY langsung dari Gameplay. Membuat sistem tiruan secara instan...");

        // 2. Buat tiruan DayTransitionManager di latar belakang secara instan
        GameObject goTransition = new GameObject("DayTransitionManager (Tiruan)");
        goTransition.AddComponent<DayTransitionManager>();

        // 3. Buat tiruan GameDataManager secara instan jika belum ada
        if (Object.FindFirstObjectByType<GameDataManager>() == null)
        {
            GameObject goData = new GameObject("GameDataManager (Tiruan)");
            goData.AddComponent<GameDataManager>();

            // Inisialisasi data awal agar tidak kosong/eror saat diakses PlayerServe
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.logistik = 10;
                GameDataManager.Instance.firstAid = 10;
            }
        }

        // 4. Buat tiruan AudioManager secara instan jika belum ada (menghindari error audio null)
        if (Object.FindFirstObjectByType<AudioManager>() == null)
        {
            GameObject goAudio = new GameObject("AudioManager (Tiruan)");
            AudioSource source = goAudio.AddComponent<AudioSource>();
            AudioManager am = goAudio.AddComponent<AudioManager>();

            // Hubungkan referensi audio source internal AudioManager kamu jika ada
            am.musicSource = source;
        }
    }

    void Start()
    {
        // 5. Paksa GameManager untuk langsung memulai permainan Hari 1 detik ini juga
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.NextDay(1);
        }
    }
}
