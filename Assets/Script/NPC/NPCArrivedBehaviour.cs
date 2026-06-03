using UnityEngine;

/// <summary>
/// StateMachineBehaviour — pasang di STATE NPC_IN di Animator Controller.
/// Otomatis memanggil OnArrivedAtService() saat animasi NPC_IN selesai.
///
/// CARA PASANG (tanpa Animation Event):
///   1. Buka Animator window
///   2. Klik state "NPC_IN"
///   3. Di Inspector, klik "Add Behaviour"
///   4. Pilih NPCArrivedBehaviour
///   Selesai — tidak perlu edit clip animasi sama sekali.
/// </summary>
public class NPCArrivedBehaviour : StateMachineBehaviour
{
    private bool sudahDipanggil = false;

    // Reset flag setiap kali masuk state NPC_IN
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sudahDipanggil = false;
    }

    // Dipanggil setiap frame selama state NPC_IN berjalan
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // normalizedTime >= 1 artinya animasi sudah selesai 100%
        if (!sudahDipanggil && stateInfo.normalizedTime >= 0.95f)
        {
            sudahDipanggil = true;

            NPC npc = animator.GetComponent<NPC>();
            if (npc != null)
            {
                Debug.Log("<color=lime>[NPCArrivedBehaviour]</color> Animasi NPC_IN selesai, memicu dialog.");
                npc.OnArrivedAtService();
            }
            else
            {
                Debug.LogWarning("[NPCArrivedBehaviour] Komponen NPC tidak ditemukan di GameObject ini!");
            }
        }
    }
}
