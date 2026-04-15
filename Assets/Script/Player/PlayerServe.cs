using UnityEngine;
using TMPro;

public class PlayerServe : MonoBehaviour
{
    [Header("Stok Sekarang (Sisa Bantuan)")]
    public int logistik;
    public int firstAid;

    [Header("Statistik Hari Ini")]
    public int totalLogistikMasuk;
    public int totalMedicMasuk;
    public int totalLogistikKeluar;
    public int totalMedicKeluar;

    [Header("Referensi UI Laporan")]
    private TextMeshProUGUI txtMasuk;  // Tarik teks laporan ke sini
    private TextMeshProUGUI txtKeluar;
    private TextMeshProUGUI txtSisa;

    public NPCQueue queue;

    // Panggil ini dari DragClone saat item masuk ke meja
    public void CatatMasuk(KebutuhanType tipe, int jumlah)
    {
        if (tipe == KebutuhanType.Logistik) {
            logistik += jumlah;
            totalLogistikMasuk += jumlah;
        } else {
            firstAid += jumlah;
            totalMedicMasuk += jumlah;
        }
        UpdateLaporanUI();
    }

    public void buttonBantuanClick()
    {
        NPC npc = queue.GetForntNPC();

        if(npc != null && npc.CekTerpenuhi(logistik, firstAid))
        {
            // Update Statistik Keluar
            totalLogistikKeluar += npc.kebutuhan.logistik;
            totalMedicKeluar += npc.kebutuhan.firstAid;

            // Update Sisa
            logistik -= npc.kebutuhan.logistik;
            firstAid -= npc.kebutuhan.firstAid;
            
            npc.TriggerKeluar(); 
            HapusItemDiMeja();
            queue.RemoveForntNPC();
            
            UpdateLaporanUI();
        }
    }

    void UpdateLaporanUI()
    {
        if (txtMasuk == null) return;

        txtMasuk.text = $"Logistik: {totalLogistikMasuk}\nMedic: {totalMedicMasuk}";
        txtKeluar.text = $"Logistik: {totalLogistikKeluar}\nMedic: {totalMedicKeluar}";
        txtSisa.text = $"Logistik: {logistik}\nMedic: {firstAid}";
    }

    void HapusItemDiMeja()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("ItemDimeja");
        foreach (GameObject item in items) { Destroy(item); }
    }
}