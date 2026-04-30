using UnityEngine;

public class ShowObject : MonoBehaviour
{
    public string title;
    [TextArea(3, 10)]
    public string[] daftarDeskripsi;   // deskripsi per hari
    public Sprite[] daftarTanggal;     // sprite per hari
    public UIManager uiManager;

    public void TriggerShow()
    {
        if (uiManager == null)
        {
            Debug.LogError("UIManager belum di-assign di ShowObject pada " + gameObject.name);
            return;
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.paper);

        // Ambil hari aktif dari PergantianKalender
        var kalender = Object.FindFirstObjectByType<PergantianKalender>();
        if (kalender != null)
        {
            int hariIndex = kalender.HariSekarang;

            if (hariIndex >= 0 && hariIndex < daftarTanggal.Length)
            {
                Sprite spriteHariIni = daftarTanggal[hariIndex];
                string deskripsiHariIni = daftarDeskripsi[hariIndex];

                uiManager.ShowInfo(title, deskripsiHariIni, spriteHariIni);
            }
        }
    }
}
