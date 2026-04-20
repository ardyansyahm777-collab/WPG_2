using System.Collections.Generic;
using UnityEngine;

public class NPCQueue : MonoBehaviour 
{
    [Header("References")]
    public RectTransform[] queuePoints;
    public GameObject npcPrefab;
    public KebutuhanGenerator generator;
    public Transform uiParent;

    public int GetTersisaHariIni() => npcTersisaHariIni;

    private List<NPC> npcList = new List<NPC>();
    private int npcTersisaHariIni;

    void Start() 
    { 
        MulaiHari(0); 
    }

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
            
            npcList.Insert(0, npc);
            UpdateQueuePosition();
        }
    }

    public void RemoveForntNPC() 
    {
        if (npcList.Count == 0) return;

        npcList.RemoveAt(npcList.Count - 1);
        npcTersisaHariIni--; // kurangi di sini, saat NPC benar-benar keluar
        UpdateQueuePosition();

        if (npcTersisaHariIni > 0)
        {
            Invoke("SpawnNPC", 2f);
        }
        else if (npcList.Count == 0)
        {
            Object.FindFirstObjectByType<GameManager>()?.NPCFinishedTurn();
            Debug.Log("<color=cyan>[NPCQueue]</color> Semua NPC selesai, hari berganti ke hari berikutnya.");
        }
    }


    public void UpdateQueuePosition() 
    {
        for(int i = 0; i < npcList.Count; i++)
        {
            if (npcList[i] == null) continue;

            int pointIndex = (queuePoints.Length - 1) - (npcList.Count - 1 - i);
            pointIndex = Mathf.Clamp(pointIndex, 0, queuePoints.Length - 1);
            
            npcList[i].SetTargetPos(queuePoints[pointIndex].anchoredPosition);
        }
    }

    public NPC GetForntNPC() => npcList.Count > 0 ? npcList[npcList.Count - 1] : null;
}
