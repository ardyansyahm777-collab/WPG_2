using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    [Header("Stok Bantuan Saat Ini (Sisa Gudang)")]
    public int logistik;
    public int firstAid;
    public int totalWargaDibantu;

    [Header("Statistik Laporan Hari Ini")]
    public int totalLogistikMasuk;
    public int totalMedicMasuk;
    public int totalLogistikKeluar;
    public int totalMedicKeluar;
    public int wargaDibantu;
    public int kuponBenarHariIni;
    public int kuponSalahHariIni;

    [Header("Hidden Metrics (Ending Tracker)")]
    public int compliancePoint = 0; 
    public int humanityPoint = 0;   
    public int corruptionPoint = 0; 

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

    public void ResetStatistikHarian()
    {
        totalLogistikMasuk = 0;
        totalMedicMasuk    = 0;
        totalLogistikKeluar = 0;
        totalMedicKeluar   = 0;
        wargaDibantu = 0;
        kuponBenarHariIni = 0;
        kuponSalahHariIni = 0;
    }

    public void TambahMetrik(int compliance, int humanity, int corruption = 0)
    {
        compliancePoint += compliance;
        humanityPoint += humanity;
        corruptionPoint += corruption;
    }
}