using UnityEngine;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance { get; private set; }

    public enum EndingType
    {
        None,
        EndingA_TheBureaucrat,      // Kepatuhan tinggi, mengabaikan emosi warga
        EndingB_TheSilentHero,      // Kemanusiaan tinggi, membantu warga terdesak
        EndingC_TheMartyr,          // Terlalu banyak melanggar aturan demi membantu orang sampai dipecat
        EndingD_TheCorruptedShore   // Menyelewengkan bantuan / Korupsi tinggi
    }

    [Header("Threshold Batas Poin Ending")]
    public int minHighCompliance = 10;
    public int minHighHumanity = 10;
    public int minHighCorruption = 5;

    [Header("Tipe Ending Yang Berhasil Dicapai")]
    public EndingType currentEnding = EndingType.None;

    [Header("Teks Deskripsi Ending (Untuk UI Win/Ending Panel)")]
    [TextArea(3, 5)] public string deskripsiEndingA = "Kamu menjalankan posko dengan kepatuhan prosedur yang ketat. Semua dokumen tercatat rapi tanpa celah, namun banyak korban yang terpaksa ditolak.";
    [TextArea(3, 5)] public string deskripsiEndingB = "Kamu mengutamakan keselamatan warga Aceh di atas lembaran kertas dokumen. Nama kamu dikenang hangat oleh para pengungsi.";
    [TextArea(3, 5)] public string deskripsiEndingC = "Pengorbananmu membantu warga tanpa dokumen berujung pada penindakan tegas dari atasan posko.";
    [TextArea(3, 5)] public string deskripsiEndingD = "Penyalahgunaan logistik dan kecacatan laporan di posko terungkap. Pantai Silent Shore meninggalkan catatan kelam.";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Dipanggil saat Hari ke-7 Selesai (WinGame) untuk mengevaluasi Ending Mana yang Didapat.
    /// </summary>
    public EndingType EvaluasiEndingHari7()
    {
        GameDataManager data = GameDataManager.Instance;
        if (data == null) return EndingType.None;

        // 1. Prioritas Evaluasi Korupsi/Penyalahgunaan Bantuan
        if (data.corruptionPoint >= minHighCorruption)
        {
            currentEnding = EndingType.EndingD_TheCorruptedShore;
        }
        // 2. Prioritas Evaluasi Kemanusiaan Tinggi (Empati ke Korban Bencana)
        else if (data.humanityPoint >= minHighHumanity && data.compliancePoint < minHighCompliance)
        {
            currentEnding = EndingType.EndingB_TheSilentHero;
        }
        // 3. Prioritas Evaluasi Kepatuhan Prosedur (Birokrat Sejati)
        else if (data.compliancePoint >= minHighCompliance)
        {
            currentEnding = EndingType.EndingA_TheBureaucrat;
        }
        // 4. Default Fallback Ending (Balancing Seimbang)
        else
        {
            currentEnding = (data.humanityPoint >= data.compliancePoint) 
                ? EndingType.EndingB_TheSilentHero 
                : EndingType.EndingA_TheBureaucrat;
        }

        Debug.Log($"<color=magenta>[EndingManager]</color> Evaluasi Ending Selesai! Ending Tercapai: {currentEnding}");
        return currentEnding;
    }

    /// <summary>
    /// Mengambil narasi teks ending yang dicapai untuk ditampilkan di Win Panel.
    /// </summary>
    public string GetDeskripsiEndingAktif()
    {
        switch (currentEnding)
        {
            case EndingType.EndingA_TheBureaucrat: return deskripsiEndingA;
            case EndingType.EndingB_TheSilentHero: return deskripsiEndingB;
            case EndingType.EndingC_TheMartyr: return deskripsiEndingC;
            case EndingType.EndingD_TheCorruptedShore: return deskripsiEndingD;
            default: return "Perjalanan posko relawan Silent Shore telah berakhir.";
        }
    }
}