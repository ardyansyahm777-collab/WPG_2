using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Loop")]
    public int currentDay = 1;
    public int maxDay     = 3;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("UI Teks")]
    public TextMeshProUGUI txtAlasanKalah;
    public TextMeshProUGUI txtHariSekarang;

    [Header("Lose Condition")]
    public int maxNPCMarah = 2;
    private int jumlahNPCMarah = 0;

    [Header("Referensi Script UI Laporan")]
    public LaporanHarianUI laporanHarianScript; 

    void Start()
    {
        if (winPanel  != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    public void NextDay(int targetHari)
    {
        currentDay = targetHari;
        Debug.Log($"<color=cyan>[GameManager]</color> Mulai Hari {currentDay}");

        UpdateHariUI();
        ResetMarah();
        PutarMusikGameplay();

        if (currentDay > maxDay)
        {
            WinGame();
            return;
        }

        // --- SISTEM ADD PASOKAN BANTUAN PER HARI ---
        KebutuhanGenerator generator = Object.FindFirstObjectByType<KebutuhanGenerator>();
        if (generator != null && GameDataManager.Instance != null)
        {
            // Reset statistik laporan harian dari nol di setiap awal hari baru
            GameDataManager.Instance.ResetStatistikHarian();

            int indexHari = currentDay - 1;
            if (indexHari < generator.daftarHari.Count)
            {
                // Ambil nilai dari konfigurasi KebutuhanGenerator
                int pasokanLogistik = generator.daftarHari[indexHari].pasokanLogistikHariIni;
                int pasokanMedic = generator.daftarHari[indexHari].pasokanMedicHariIni;

                // Masukkan data bantuan ke dalam penyimpanan pusat data
                GameDataManager.Instance.logistik = pasokanLogistik;
                GameDataManager.Instance.firstAid = pasokanMedic;
                GameDataManager.Instance.totalLogistikMasuk = pasokanLogistik;
                GameDataManager.Instance.totalMedicMasuk = pasokanMedic;
            }
        }

        // Sinkronisasi data ke UI utama gameplay setelah mendapatkan bantuan harian
        Object.FindFirstObjectByType<PlayerServe>()?.SinkronisasiDataPusatKeUI();

        // Spawn NPC hari ini
        Object.FindFirstObjectByType<NPCQueue>()?.MulaiHari(currentDay - 1);
    }

    /// <summary>Dipanggil NPCQueue saat shift selesai.</summary>
    public void NPCFinishedTurn()
    {
        Debug.Log("<color=orange>[GameManager]</color> Shift selesai. Membuka laporan harian.");
        
        // // Membuka UI laporan harian terlebih dahulu sebelum memicu transisi
        // if (laporanHarianScript != null)
        // {
        //     laporanHarianScript.TampilkanLaporan();
        // }
        SelesaiTampilkanLaporan(); // Fallback langsung jalan jika lupa memasang referensi UI
        
    }

    /// <summary>Dipanggil oleh LaporanHarianUI setelah tombol lanjut ditekan.</summary>
    public void SelesaiTampilkanLaporan()
    {
        int hariBerikutnya = currentDay + 1;

        if (DayTransitionManager.Instance != null)
            DayTransitionManager.Instance.MulaiTransisi(hariBerikutnya);
        else
            NextDay(hariBerikutnya);
    }

    void PutarMusikGameplay()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.background != null)
        {
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.musicSource.clip = AudioManager.Instance.background;
            AudioManager.Instance.musicSource.loop = true;
            AudioManager.Instance.musicSource.Play();
        }
    }

    void WinGame()
    {
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void NPCMarah()
    {
        jumlahNPCMarah++;
        if (jumlahNPCMarah >= maxNPCMarah) LoseGame();
    }

    public void ResetMarah() => jumlahNPCMarah = 0;

    void LoseGame()
    {
        if (txtAlasanKalah != null)
            txtAlasanKalah.text = "Kamu dipecat!\nBanyak warga yang marah karena kesalahan layananmu.";
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void UpdateHariUI()
    {
        if (txtHariSekarang != null)
            txtHariSekarang.text = $"Hari {currentDay}";
    }

    public void OnWinContinue()  { Time.timeScale = 1f; UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); }
    public void OnLoseRetry()    { Time.timeScale = 1f; UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); }
    public void OnLoseMainMenu() { Time.timeScale = 1f; UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); }
}