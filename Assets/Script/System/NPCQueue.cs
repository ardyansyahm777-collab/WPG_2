using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Antrean NPC dengan sistem Animator.
/// NPC bergerak via animasi (NPC_IN), dan dialog dipicu via Animation Event,
/// bukan lewat pengecekan jarak di Update().
/// </summary>
public class NPCQueue : MonoBehaviour
{
    // =============================================
    // INSPECTOR
    // =============================================
    [Header("Prefab & Spawn")]
    public GameObject npcPrefab;

    [Tooltip("Assign 'npc_container' dari Hierarchy (RectTransform dalam Canvas).")]
    public RectTransform npcContainer;

    [Tooltip("Titik awal NPC di-spawn. Assign Point_1 (posisi luar layar) agar NPC tidak tiba-tiba muncul di tengah.")]
    public RectTransform spawnPoint;

    [Header("Referensi")]
    public KebutuhanGenerator generator;

    [Header("Pengaturan Spawn")]
    [Tooltip("Jeda sebelum NPC berikutnya muncul setelah NPC sebelumnya pergi (detik).")]
    public float spawnInterval = 1.5f;

    // =============================================
    // PRIVATE STATE
    // =============================================
    private NPC npcAktif = null;   // hanya 1 NPC aktif sekaligus
    private int totalSpawn  = 0;
    private int targetNPC   = 0;
    private bool shiftAktif = false;

    // =============================================
    // UNITY LIFECYCLE
    // =============================================
    void Start()
    {
        // SOLUSI UTAMA: MulaiHari(0) DIHAPUS dari sini!
        // Sekarang, antrean NPC hanya akan dipanggil secara presisi oleh CutsceneManager 
        // setelah layar hitam transisi selesai memudar (Fade In).
    }

    // =============================================
    // SPAWN
    // =============================================
    void SpawnNPC()
    {
        if (npcPrefab == null || generator == null) return;
        if (totalSpawn >= targetNPC) return;
        if (npcAktif != null) return; // masih ada NPC aktif

        Transform parent = ResolveParent();
        if (parent == null)
        {
            Debug.LogError("[NPCQueue] Parent tidak ditemukan! Assign npcContainer di Inspector.");
            return;
        }

        GameObject obj = Instantiate(npcPrefab, parent);

        NPC npc = obj.GetComponent<NPC>();
        if (npc == null)
        {
            Debug.LogError("[NPCQueue] Prefab tidak punya komponen NPC.cs!");
            Destroy(obj);
            return;
        }

        npc.SetKebutuhan(generator.GetRandomKebutuhan());
        npc.SetVisual(generator.GetRandomSprite());

        RectTransform npcRect = obj.GetComponent<RectTransform>();
        if (npcRect != null)
        {
            if (spawnPoint != null)
                npcRect.anchoredPosition = spawnPoint.anchoredPosition;
            else
                Debug.LogWarning("[NPCQueue] spawnPoint belum di-assign! NPC mungkin muncul di posisi salah. Assign Point_1 di Inspector.");
        }

        npcAktif = npc;
        totalSpawn++;

        Debug.Log($"<color=cyan>[NPCQueue]</color> Spawn NPC #{totalSpawn}/{targetNPC} di posisi {(spawnPoint != null ? spawnPoint.anchoredPosition.ToString() : "default")}");

        Animator anim = obj.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("NPC_IN");
        else
            Debug.LogWarning("[NPCQueue] NPC tidak punya Animator! Pasang Animator Controller.");
    }

    IEnumerator SpawnSetelahDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNPC();
    }

    Transform ResolveParent()
    {
        if (npcContainer != null) return npcContainer;

        GameObject found = GameObject.Find("npc_container");
        if (found != null)
        {
            npcContainer = found.GetComponent<RectTransform>();
            if (npcContainer != null) return npcContainer; // Sudah diperbaiki dari 'parent' menjadi 'npcContainer'
        }

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        return canvas != null ? canvas.transform : null;
    }

    // =============================================
    // PUBLIC API
    // =============================================
    public NPC GetForntNPC() => npcAktif;

    public void RemoveForntNPC()
    {
        npcAktif = null;
        Debug.Log($"<color=cyan>[NPCQueue]</color> NPC selesai. Total spawn: {totalSpawn}/{targetNPC}");

        if (CekShiftSelesai()) return;

        StartCoroutine(SpawnSetelahDelay(spawnInterval));
    }

    // =============================================
    // MULTI-HARI
    // =============================================
    public void MulaiHari(int indexHari)
    {
        if (generator == null)
        {
            Debug.LogError("[NPCQueue] generator belum di-assign!");
            return;
        }

        if (npcAktif != null)
        {
            Destroy(npcAktif.gameObject);
            npcAktif = null;
        }

        totalSpawn = 0;
        shiftAktif = true;

        generator.indexHariSekarang = indexHari;
        targetNPC = generator.GetTargetNPC();

        Debug.Log($"<color=cyan>[NPCQueue]</color> Mulai hari index={indexHari}, target={targetNPC} NPC");

        SpawnNPC();
    }

    bool CekShiftSelesai()
    {
        if (!shiftAktif) return true;
        if (totalSpawn < targetNPC || npcAktif != null) return false;

        shiftAktif = false;
        Debug.Log("<color=orange>[NPCQueue]</color> Semua NPC selesai. Memberi tahu GameManager.");
        Object.FindFirstObjectByType<GameManager>()?.NPCFinishedTurn();
        return true;
    }
}