using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    [Header("Stok Bantuan Saat Ini (Sisa Gudang)")]
    public int logistik;
    public int firstAid;

    [Header("Statistik Akumulasi Total")]
    public int totalWargaDibantu;

    [Header("Statistik Laporan Hari Ini")]
    public int totalLogistikMasuk;
    public int totalMedicMasuk;
    public int totalLogistikKeluar;
    public int totalMedicKeluar;
    public int wargaDibantu;
    public int kuponBenarHariIni;
    public int kuponSalahHariIni;

    [Header("Akumulasi Total Keputusan (Untuk Ending)")]
    public int totalKuponBenar;
    public int totalKuponSalah;

    [Header("Hidden Metrics (Ending Tracker)")]
    public int compliancePoint = 0; // Kepatuhan Prosedur Posko
    public int humanityPoint = 0;   // Kemanusiaan / Empati
    public int corruptionPoint = 0; // Penyelewengan / Korupsi Bantuan

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Dipanggil setiap pergantian hari di GameManager untuk mereset angka statistik harian.
    /// </summary>
    public void ResetStatistikHarian()
    {
        // Simpan dulu akumulasi total sebelum di-reset harian
        totalKuponBenar += kuponBenarHariIni;
        totalKuponSalah += kuponSalahHariIni;

        totalLogistikMasuk = 0;
        totalMedicMasuk    = 0;
        totalLogistikKeluar = 0;
        totalMedicKeluar   = 0;
        wargaDibantu = 0;
        kuponBenarHariIni = 0;
        kuponSalahHariIni = 0;
    }

    /// <summary>
    /// Menambahkan poin naratif hidden untuk menentukan branching ending game di Hari 7.
    /// </summary>
    public void TambahMetrik(int compliance, int humanity, int corruption = 0)
    {
        compliancePoint += compliance;
        humanityPoint += humanity;
        corruptionPoint += corruption;

        Debug.Log($"<color=yellow>[GameDataManager]</color> Point Updated -> Compliance: {compliancePoint}, Humanity: {humanityPoint}, Corruption: {corruptionPoint}");
    }
}