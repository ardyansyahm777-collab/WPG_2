using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    [Header("Stok Bantuan Saat Ini (Di Meja)")]
    public int logistik;
    public int firstAid;

    [Header("Statistik Laporan Hari Ini")]
    public int totalLogistikMasuk;
    public int totalMedicMasuk;
    public int totalLogistikKeluar;
    public int totalMedicKeluar;
    public int wargaBerhasilDibantu; // Tambahan data warga yang sudah dibantu

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Opsional: Hilangkan tanda komentar jika data ingin persisten antar scene
        // DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Membersihkan data statistik harian setiap kali hari baru dimulai.
    /// </summary>
    public void ResetStatistikHarian()
    {
        totalLogistikMasuk = 0;
        totalMedicMasuk = 0;
        totalLogistikKeluar = 0;
        totalMedicKeluar = 0;
        wargaBerhasilDibantu = 0;
    }
}