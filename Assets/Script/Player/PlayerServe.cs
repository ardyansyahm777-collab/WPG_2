using UnityEngine;

public class PlayerServe : MonoBehaviour
{
    public int logistik;
    public int firstAid;

    public NPCQueue queue;


    public void buttonBantuanClick()
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
