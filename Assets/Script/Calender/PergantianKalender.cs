using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; 

public class PergantianKalender : MonoBehaviour
{
    public Image calendarImage;
    public Sprite[] daftarTanggal;
    public TextMeshProUGUI tanggalText;
    public GameObject laporanUI;

    // Tidak perlu lagi memakai variabel lokal 'hariSekarang = 0' yang gampang ke-reset

    void Start()
    {
        // Beri delay sedikit agar GameManager selesai inisialisasi NextDay()
        StartCoroutine(InitDelay());
    }

    IEnumerator InitDelay()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateKalender();
    }

    public void UpdateKalender()
    {
        // 1. Ambil data hari riil dari GameManager yang sedang aktif
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            // Dikurangi 1 karena indeks array/list dimulai dari 0, sedangkan currentDay dimulai dari 1
            int indeksHari = gm.currentDay - 1; 

            // 2. Validasi apakah indeks aman dan pasang sprite yang sesuai
            if (indeksHari >= 0 && indeksHari < daftarTanggal.Length && calendarImage != null)
            {
                calendarImage.sprite = daftarTanggal[indeksHari];
                Debug.Log($"[Kalender] Berhasil update ke gambar hari index: {indeksHari} (Hari {gm.currentDay})");
            }
            else
            {
                Debug.LogWarning("[Kalender] Indeks hari di luar jangkauan daftarTanggal!");
            }

            // 3. Update teks tampilan jika ada
            if (tanggalText != null)
                tanggalText.text = $"Hari ke-{gm.currentDay}";
        }
        else
        {
            Debug.LogError("[Kalender] GameManager tidak ditemukan di scene!");
        }
    }
}