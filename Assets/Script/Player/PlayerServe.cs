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
            queue = Object.FindFirstObjectByType<NPCQueue>();

        SinkronisasiDataPusatKeUI();
    }

    /// <summary>
    /// Sinkronisasi visual text UI dengan data di GameDataManager.
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

        // Hitung berapa item yang saat ini ada di meja berdasarkan tag
        int logistikDiMeja = 0;
        int firstAidDiMeja = 0;

        GameObject[] itemsDiMeja = GameObject.FindGameObjectsWithTag("ItemDimeja");
        foreach (GameObject item in itemsDiMeja)
        {
            DragClone drag = item.GetComponent<DragClone>();
            if (drag == null) continue;

            if (drag.tipeItem == KebutuhanType.Logistik)
                logistikDiMeja += drag.jumlahItem;
            else
                firstAidDiMeja += drag.jumlahItem;
        }

        // Bandingkan item di meja dengan kebutuhan NPC
        bool tepat = (logistikDiMeja == npc.kebutuhan.logistik &&
                      firstAidDiMeja  == npc.kebutuhan.firstAid);

        if (tepat)
        {
            // Kurangi stok gudang sesuai yang diberikan
            data.logistik -= npc.kebutuhan.logistik;
            data.firstAid -= npc.kebutuhan.firstAid;

            // Catat ke statistik laporan
            data.totalLogistikKeluar += npc.kebutuhan.logistik;
            data.totalMedicKeluar    += npc.kebutuhan.firstAid;
            data.wargaDibantu++;
            data.totalWargaDibantu++;

            npc.TriggerKeluar();
            HapusItemDiMeja();
            queue.RemoveForntNPC();
        }
        else
        {
            // Salah: stok TIDAK berkurang, item meja hanya dihapus
            npc.WrongResponse();
            HapusItemDiMeja();
        }

        SinkronisasiDataPusatKeUI();
    }

    void HapusItemDiMeja()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("ItemDimeja");
        foreach (GameObject item in items) Destroy(item);
    }
}