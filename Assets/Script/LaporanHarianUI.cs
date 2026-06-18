using UnityEngine;
using TMPro;

public class LaporanHarianUI : MonoBehaviour
{
    [Header("Panel Utama")]
    public GameObject panelLaporan;

    [Header("Referensi UI Text (Sesuai Gambar Layout)")]
    public TextMeshProUGUI txtMasuk;
    public TextMeshProUGUI txtKeluar;
    public TextMeshProUGUI txtSisa;
    public TextMeshProUGUI txtWargaDibantu; // Menampilkan total warga yang berhasil dibantu hari ini

    /// <summary>
    /// Membuka panel laporan harian dan mengambil data terbaru dari GameDataManager.
    /// </summary>
    public void TampilkanLaporan()
    {
        panelLaporan.SetActive(true);
        Time.timeScale = 0f; // Menghentikan waktu gameplay (pause) saat laporan aktif

        // GameDataManager data = GameDataManager.Instance;
        // if (data != null)
        // {
        //     txtMasuk.text  = $"Logistik : {data.totalLogistikMasuk}\nMedic    : {data.totalMedicMasuk}";
        //     txtKeluar.text = $"Logistik : {data.totalLogistikKeluar}\nMedic    : {data.totalMedicKeluar}";
        //     txtSisa.text   = $"Logistik : {data.logistik}\nMedic    : {data.firstAid}";
            
        //     if (txtWargaDibantu != null)
        //     {
        //         txtWargaDibantu.text = $"Warga Yang Dibantu:\n{data.wargaBerhasilDibantu} Orang";
        //     }
        // }
    }

    /// <summary>
    /// Dipanggil oleh Button "LANJUT" di dalam UI Laporan Harian.
    /// </summary>
    public void TombolLanjutKlik()
    {
        Time.timeScale = 1f; // Mengaktifkan kembali waktu game
        panelLaporan.SetActive(false);

        // Beritahu GameManager untuk melanjutkan siklus transisi hari berikutnya
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.SelesaiTampilkanLaporan();
        }
    }
}