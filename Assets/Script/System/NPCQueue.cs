using System.Collections.Generic;
using UnityEngine;

public class NPCQueue : MonoBehaviour 
{
    [Header("References")]
    public RectTransform[] queuePoints; // Point_1 (Masuk), Point_2 (Tengah/Meja)
    public GameObject npcPrefab;
    public KebutuhanGenerator generator;
    public Transform uiParent;

    [Header("Debug Info")]
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
            
            // Masukkan ke urutan 0 (paling belakang dalam list logic)
            npcList.Insert(0, npc);
            npcTersisaHariIni--; 
            UpdateQueuePosition();
        }

        // Cek apakah semua NPC hari ini sudah selesai
        if (npcTersisaHariIni == 0 && npcList.Count == 0)
        {
            Object.FindFirstObjectByType<PergantianKalender>().NPCFinishedTurn();
            Debug.Log("<color=cyan>[NPCQueue]</color> Semua NPC selesai, hari berganti.");
        }   
    }

    public void RemoveForntNPC() 
    {
        if (npcList.Count == 0) return;
        
        // NPC yang dihapus adalah yang paling depan (indeks terakhir)
        npcList.RemoveAt(npcList.Count - 1);
        UpdateQueuePosition();

        if (npcTersisaHariIni > 0)
        {
            Invoke("SpawnNPC", 2f);
        }
        else if (npcList.Count == 0)
        {
            // Semua NPC hari ini sudah selesai
            Object.FindFirstObjectByType<PergantianKalender>().NPCFinishedTurn();
            Debug.Log("<color=cyan>[NPCQueue]</color> Semua NPC selesai, hari berganti ke hari berikutnya.");
        }
    }

    public void UpdateQueuePosition() 
    {
        for(int i = 0; i < npcList.Count; i++)
        {
            // Indeks terakhir di npcList = queuePoints terakhir (depan meja)
            int pointIndex = (queuePoints.Length - 1) - (npcList.Count - 1 - i);
            pointIndex = Mathf.Clamp(pointIndex, 0, queuePoints.Length - 1);
            
            npcList[i].SetTargetPos(queuePoints[pointIndex].anchoredPosition);
        }
    }

    public NPC GetForntNPC() => npcList.Count > 0 ? npcList[npcList.Count - 1] : null;
}