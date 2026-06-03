using UnityEngine;
using TMPro;

public class PlayerServe : MonoBehaviour
{
    [Header("Referensi UI HUD di Scene Gameplay")]
    public TextMeshProUGUI txtMasuk;
    public TextMeshProUGUI txtKeluar;
    public TextMeshProUGUI txtSisa;

    [Header("Referensi Antrean")]
    public NPCQueue queue;

    void Start()
    {
        if (queue == null)
        {
            queue = Object.FindFirstObjectByType<NPCQueue>();
        }
        SinkronisasiDataPusatKeUI();
    }

    /// <summary>
    /// Sinkronisasi visual text UI dengan data yang ada di GameDataManager pusat.
    /// </summary>
    public void SinkronisasiDataPusatKeUI()
    {
        if (txtMasuk == null || GameDataManager.Instance == null) return;

        GameDataManager data = GameDataManager.Instance;
        txtMasuk.text  = $"Logistik: {data.totalLogistikMasuk}\nMedic: {data.totalMedicMasuk}";
        txtKeluar.text = $"Logistik: {data.totalLogistikKeluar}\nMedic: {data.totalMedicKeluar}";
        txtSisa.text   = $"Logistik: {data.logistik}\nMedic: {data.firstAid}";
    }

    // =============================================
    // TOMBOL BANTU
    // =============================================
    public void buttonBantuanClick()
    {
        if (queue == null) queue = Object.FindFirstObjectByType<NPCQueue>();

        NPC npc = queue != null ? queue.GetForntNPC() : null;

        if (npc == null) return;

        if (!npc.SudahTriggerDialog()) return;

        GameDataManager data = GameDataManager.Instance;
        if (data == null) return;

        // Pengecekan kecocokan stok menggunakan data terpusat dari GameDataManager
        bool tepat = (data.logistik == npc.kebutuhan.logistik && data.firstAid == npc.kebutuhan.firstAid);

        if (tepat)
        {
            // Tambahkan catatan keluar ke data pusat
            data.totalLogistikKeluar += npc.kebutuhan.logistik;
            data.totalMedicKeluar    += npc.kebutuhan.firstAid;

            // Kurangi sisa stok barang bantuan di data pusat
            data.logistik -= npc.kebutuhan.logistik;
            data.firstAid -= npc.kebutuhan.firstAid;

            // Tambahkan statistik warga yang berhasil dibantu
            data.wargaBerhasilDibantu++;

            npc.TriggerKeluar();
            HapusItemDiMeja();
            queue.RemoveForntNPC();
        }
        else
        {
            npc.WrongResponse();
            HapusItemDiMeja();
            
            // Konsekuensi salah penanganan bantuan: mengosongkan meja kembali
            data.logistik = 0;
            data.firstAid = 0;
        }

        SinkronisasiDataPusatKeUI();
    }

    // =============================================
    // ITEM MASUK KE MEJA (dipanggil DragClone)
    // =============================================
    public void CatatMasuk(KebutuhanType tipe, int jumlah)
    {
        GameDataManager data = GameDataManager.Instance;
        if (data == null) return;

        if (tipe == KebutuhanType.Logistik)
        {
            data.logistik           += jumlah;
            data.totalLogistikMasuk += jumlah;
        }
        else
        {
            data.firstAid        += jumlah;
            data.totalMedicMasuk += jumlah;
        }
        SinkronisasiDataPusatKeUI();
    }

    void HapusItemDiMeja()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("ItemDimeja");
        foreach (GameObject item in items) Destroy(item);
    }
}