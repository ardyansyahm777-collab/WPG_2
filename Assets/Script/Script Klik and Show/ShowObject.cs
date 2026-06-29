using UnityEngine;

public class ShowObject : MonoBehaviour
{
    // Membuat struktur data berpasangan antara Sprite dan Deskripsi khusus
    [System.Serializable]
    public struct DataHarianKalender
    {
        public Sprite spriteHariIni;
        [TextArea(3, 10)]
        public string deskripsiHariIni;
    }

    public string title;

    [Header("Gunakan ini untuk objek biasa (Bukan Kalender)")]
    [TextArea(3, 10)]
    public string description;
    public Sprite objectSprite;

    [Header("Khusus Objek Kalender")]
    [Tooltip("Isi daftar gambar dan deskripsi unik untuk tiap hari di sini")]
    public DataHarianKalender[] dataHarianKalender;
    
    // Referensi ke UIManager
    public UIManager uiManager;

    public void TriggerShow()
    {
        if (uiManager != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.paper);

            // JIKA OBJEK INI ADALAH KALENDER
            if (gameObject.name == "calendar")
            {
                // 1. Ambil data hari aktif dari GameManager
                GameManager gm = Object.FindFirstObjectByType<GameManager>();
                if (gm != null)
                {
                    int indeksHari = gm.currentDay - 1; // Konversi hari riil ke indeks array (0, 1, 2...)

                    // 2. Validasi apakah data hari ini tersedia di dalam array dataHarianKalender
                    if (indeksHari >= 0 && indeksHari < dataHarianKalender.Length)
                    {
                        // Ambil sprite dan deskripsi spesifik hari tersebut
                        Sprite spriteHariIni = dataHarianKalender[indeksHari].spriteHariIni;
                        string deskripsiHariIni = dataHarianKalender[indeksHari].deskripsiHariIni;

                        // Tampilkan ke UI secara dinamis
                        uiManager.ShowInfo(title, deskripsiHariIni, spriteHariIni);
                    }
                    else
                    {
                        Debug.LogWarning($"[ShowObject] Data untuk Hari {gm.currentDay} belum diisi di array dataHarianKalender!");
                        // Fallback ke data default jika lupa isi
                        uiManager.ShowInfo(title, description, objectSprite);
                    }
                }
                else
                {
                    Debug.LogError("[ShowObject] GameManager tidak ditemukan!");
                    uiManager.ShowInfo(title, description, objectSprite);
                }
            }
            else
            {
                // JIKA BUKAN KALENDER (Objek biasa, pakai data default)
                uiManager.ShowInfo(title, description, objectSprite);
            }
        }
        else
        {
            Debug.LogError("UIManager belum di-assign di ShowObject pada " + gameObject.name);
        }
    }
}