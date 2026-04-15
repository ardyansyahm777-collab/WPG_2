using System.Collections.Generic;
using UnityEngine;

public class NPCQueue : MonoBehaviour 
{
    public RectTransform[] queuePoints;
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
        SpawnNPC(); // Munculkan 1 NPC pertama
    }

    public void SpawnNPC() 
    {
        if (npcList.Count < queuePoints.Length && npcTersisaHariIni > 0) 
        {
            GameObject obj = Instantiate(npcPrefab, uiParent);
            NPC npc = obj.GetComponent<NPC>();
            npc.SetKebutuhan(generator.GetRandomKebutuhan());
            npc.SetVisual(generator.GetRandomSprite());
            npcList.Insert(0, npc);
            npcTersisaHariIni--; 
            UpdateQueuePosition();
        }
    }

    public void RemoveForntNPC() 
    {
        if (npcList.Count == 0) return;
        npcList.RemoveAt(npcList.Count - 1);
        UpdateQueuePosition();
        Invoke("SpawnNPC", 2f); // NPC selanjutnya muncul setelah jeda
    }

    public void UpdateQueuePosition() 
    {
        for(int i = 0; i < npcList.Count; i++)
            npcList[i].SetTargetPos(queuePoints[i].anchoredPosition);
    }

    public NPC GetForntNPC() => npcList.Count > 0 ? npcList[npcList.Count - 1] : null;
}