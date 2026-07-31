using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Antrean NPC dengan sistem Animator.
/// NPC bergerak via animasi (NPC_IN), dan dialog dipicu via Animation Event,
/// bukan lewat pengecekan jarak di Update().
/// </summary>
[System.Serializable]
public class StoryNPCSchedule
{
    [Tooltip("Hari ke berapa NPC ini muncul (1 = Hari 1, dst - sesuai penomoran 'Hari X' di game).")]
    public int hari;

    [Tooltip("Urutan spawn ke berapa di hari itu (0 = NPC pertama yang muncul hari itu, 1 = NPC kedua, dst).")]
    public int urutanSpawnKe = 0;

    [Tooltip("Prefab Story NPC yang datanya (Kupon/KTP/Voucher/Dialog) sudah diisi manual lewat Inspector.")]
    public GameObject npcPrefabCerita;
}

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
    public KuponGenerator kuponGenerator;
    public VoucherGenerator voucherGenerator;

    [Header("Story NPC (Terjadwal)")]
    [Tooltip("NPC cerita (Teuku Rahman, dr. Maya, dll) yang muncul di hari & urutan tertentu, menyelip di antara NPC acak.")]
    public List<StoryNPCSchedule> jadwalStoryNPC = new List<StoryNPCSchedule>();

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
        if (generator == null) return;
        if (totalSpawn >= targetNPC) return;
        if (npcAktif != null) return;

        Transform parent = ResolveParent();
        if (parent == null) return;

        GameObject prefabUntukSpawn = ResolvePrefabSpawnBerikutnya();
        if (prefabUntukSpawn == null) return;

        GameObject obj = Instantiate(prefabUntukSpawn, parent);
        NPC npc = obj.GetComponent<NPC>();

        if (npc == null)
        {
            Destroy(obj);
            return;
        }

        if (npc.isStoryNPC)
        {
            // Story NPC datanya (Kebutuhan/Kupon/KTP/Voucher/Dialog) SUDAH diisi
            // manual lewat Inspector prefab - jangan ditimpa oleh generator acak.
            Debug.Log($"<color=magenta>[NPCQueue]</color> Spawn Story NPC terjadwal: {obj.name}");
        }
        else
        {
            // Set Kebutuhan Logistik/Medic bawaan[cite: 21]
            npc.SetKebutuhan(generator.GetRandomKebutuhan());

            // Ambil Gambar & Dokumen yang SINKRON dari DocumentDataGenerator
            if (DocumentDataGenerator.Instance != null)
            {
                // 1. Ambil Profile Visual (Sprite + Gender + Usia)
                NPCRandomProfile profile = DocumentDataGenerator.Instance.GetRandomProfile();

                if (profile != null)
                {
                    // Set Visual Gambar NPC
                    npc.SetVisual(profile.avatarSprite);

                    // Generate Kupon & KTP SEKALIGUS dari nama/tanggal lahir yang SAMA,
                    // supaya kedua dokumen konsisten (bisa dibandingkan pemain).
                    DocumentDataGenerator.Instance.GenerateNPCDocuments(
                        profile, generator.indexHariSekarang + 1,
                        out KuponInfo matchedKupon, out KTPInfo matchedKtp);

                    npc.SetKupon(matchedKupon);
                    npc.SetKTP(matchedKtp);
                }
            }
            else
            {
                // Fallback jika DocumentDataGenerator tidak ada[cite: 21]
                npc.SetVisual(generator.GetRandomSprite());
                npc.SetKupon(kuponGenerator.Generate(generator.indexHariSekarang + 1));
            }

            // Generate voucher (kupon sembako/medis) sebagai dokumen referensi.
            // Nomor registrasinya WAJIB sama dengan Kupon (Kartu Bantuan) di atas.
            if (voucherGenerator != null && npc.kupon != null)
            {
                npc.SetVoucher(voucherGenerator.Generate(generator.indexHariSekarang + 1, npc.kupon.nomorRegistrasi));
            }
        }

        RectTransform npcRect = obj.GetComponent<RectTransform>();
        if (npcRect != null && spawnPoint != null)
        {
            npcRect.anchoredPosition = spawnPoint.anchoredPosition;
        }

        npcAktif = npc;
        totalSpawn++;

        Animator anim = obj.GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("NPC_IN");
    }

    /// <summary>
    /// Cek apakah spawn slot berikutnya (hari sekarang + urutan spawn ke berapa)
    /// punya Story NPC terjadwal. Kalau ada, pakai prefab itu. Kalau tidak,
    /// fallback ke npcPrefab generic (NPC acak).
    /// </summary>
    private GameObject ResolvePrefabSpawnBerikutnya()
    {
        int hariSekarang = generator.indexHariSekarang + 1;

        if (jadwalStoryNPC != null)
        {
            foreach (StoryNPCSchedule jadwal in jadwalStoryNPC)
            {
                if (jadwal.hari == hariSekarang && jadwal.urutanSpawnKe == totalSpawn && jadwal.npcPrefabCerita != null)
                {
                    return jadwal.npcPrefabCerita;
                }
            }
        }

        return npcPrefab;
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
            if (npcContainer != null) return npcContainer; 
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

        // Jika NPC belum mencapai target hari ini, jadwalkan spawn berikutnya
        if (totalSpawn < targetNPC)
        {
            StartCoroutine(SpawnSetelahDelay(spawnInterval));
        }
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

    /// <summary>
    /// Mengecek apakah seluruh NPC pada hari ini sudah selesai dilayani dan keluar dari game.
    /// </summary>
    public bool CekShiftSelesai()
    {
        if (!shiftAktif) return true;

        // Shift benar-benar selesai jika target jumlah NPC sudah terpenuhi DAN sudah tidak ada NPC aktif di meja counter
        if (totalSpawn >= targetNPC && npcAktif == null)
        {
            shiftAktif = false;
            return true;
        }
        return false;
    }
}