using System.Collections.Generic;
using UnityEngine;

public class NPCQueue : MonoBehaviour
{
    public Transform[] queuePoints;
    public GameObject npcPrefab;
    public KebutuhanGenerator generator;

    private List<NPC> npcList = new List<NPC>();

    public void SpawnNPC()
    {
        GameObject obj = Instantiate(npcPrefab, queuePoints[0].position, Quaternion.identity);
        NPC npc = obj.GetComponent<NPC>();

        npc.SetKebutuhan(generator.GetRandom());

        npcList.Insert(0, npc);
        UpdateQueuePosition();
    }

    void UpdateQueuePosition()
    {
        for(int i = 0; i < npcList.Count; i++)
        {
            npcList[i].transform.position = queuePoints[i].position;
        }
    }

    public NPC GetForntNPC()
    {
        if(npcList.Count > 0)
            return npcList[npcList.Count - 1];

        return null;
    }

    public void RemoveForntNPC()
    {
        if (npcList.Count == 0) return;

        NPC fornt = npcList[npcList.Count - 1];
        npcList.RemoveAt(npcList.Count - 1);

        Destroy(fornt.gameObject);

        UpdateQueuePosition();
    }
}
