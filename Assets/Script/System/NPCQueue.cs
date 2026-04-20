using System.Collections.Generic;
using UnityEngine;

public class NPCQueue : MonoBehaviour 
{
    public RectTransform[] queuePoints; // Point_1 (Masuk), Point_2 (Tengah/Meja)
    public GameObject npcPrefab;
    public KebutuhanGenerator generator;
    public Transform uiParent;

    private List<NPC> npcList = new List<NPC>();
    private int npcTersisaHariIni;

    void Start() { MulaiHari(0); }

    public void MulaiHari(int index)
    {
        if (index >= generator.daftarHari.Count) return;
        generator.indexHariSekarang = index;
        npcTersisaHariIni = generator.GetTotalNPC();
        SpawnNPC(); 
    }

    public void SpawnNPC() 
    {
        if (npcList.Count < queuePoints.Length && npcTersisaHariIni > 0) 
        {
            GameObject obj = Instantiate(npcPrefab, uiParent);
            NPC npc = obj.GetComponent<NPC>();
            
            npc.SetKebutuhan(generator.GetRandomKebutuhan());
            npc.SetVisual(generator.GetRandomSprite());
            
            // Masukkan ke urutan 0 (paling belakang dalam list logic)
            npcList.Insert(0, npc);
            npcTersisaHariIni--; 
            UpdateQueuePosition();
        }
    }

    public void RemoveForntNPC() 
    {
        if (npcList.Count == 0) return;
        
        // NPC yang dihapus adalah yang paling depan (indeks terakhir)
        npcList.RemoveAt(npcList.Count - 1);
        UpdateQueuePosition();
        Invoke("SpawnNPC", 2f); 
    }

    public void UpdateQueuePosition() 
    {
        for(int i = 0; i < npcList.Count; i++)
        {
            // Logika: Indeks terakhir di npcList mendapatkan queuePoints terakhir (depan meja)
            // Indeks 0 di npcList mendapatkan queuePoints yang lebih belakang
            int pointIndex = (queuePoints.Length - 1) - (npcList.Count - 1 - i);
            
            // Cegah error index out of range
            pointIndex = Mathf.Clamp(pointIndex, 0, queuePoints.Length - 1);
            
            npcList[i].SetTargetPos(queuePoints[pointIndex].anchoredPosition);
        }
    }

    public NPC GetForntNPC() => npcList.Count > 0 ? npcList[npcList.Count - 1] : null;
}