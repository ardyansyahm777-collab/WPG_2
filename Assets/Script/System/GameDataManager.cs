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

    void Awake()
    {
        // Jika sudah ada Instance lain yang hidup dari hari sebelumnya...
        if (Instance != null && Instance != this)
        {
            // ...hancurkan objek yang baru lahir ini agar data lama di Instance yang asli TIDAK tertimpa!
            Destroy(gameObject);
            return;
        }
        
        // Jika ini adalah pertama kali game dijalankan (Hari 1)
        Instance = this;
        
        // PERINTAH UTAMA: Amankan objek ini agar tidak hancur saat LoadScene hari berikutnya
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Membersihkan data statistik harian setiap kali hari baru dimulai.
    /// Stok (logistik & firstAid) diisi ulang oleh GameManager setelah fungsi ini.
    /// </summary>
    public void ResetStatistikHarian()
    {
        totalLogistikMasuk = 0;
        totalMedicMasuk    = 0;
        totalLogistikKeluar = 0;
        totalMedicKeluar   = 0;
        wargaDibantu = 0;
    }
}