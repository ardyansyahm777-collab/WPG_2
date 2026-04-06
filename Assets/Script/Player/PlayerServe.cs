using UnityEngine;

public class PlayerServe : MonoBehaviour
{
    public int logistik;
    public int firstAid;

    public NPCQueue queue;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            NPC npc = queue.GetForntNPC();

            if(npc != null && npc.CekTerpenuhi(logistik, firstAid))
            {
                logistik -= npc.kebutuhan.logistik;
                firstAid -= npc.kebutuhan.firstAid;

                queue.RemoveForntNPC();
            }
        }
    }
}
