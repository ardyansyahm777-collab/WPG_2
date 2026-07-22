using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NPC : MonoBehaviour
{
    // =============================================
    // DATA & DIALOG
    // =============================================
    [Header("Data & Dialog")]
    public KebutuhanSet kebutuhan;
    public DialogData daftarDialog;
    public KuponInfo kupon; //New: KuponInfo untuk NPC ini

    // =============================================
    // UI REFERENCES
    // =============================================
    [Header("UI References")]
    public GameObject bubbleChatObject;
    public TextMeshProUGUI bubbleChatText;
    public Image avatarImage;
    public TextMeshProUGUI logistikText;
    public TextMeshProUGUI firstAidText;

    // =============================================
    // SETTINGS
    // =============================================
    [Header("Settings")]
    public float moveSpeed = 8f; // tidak dipakai jika pakai Animator, tapi dibiarkan agar tidak error

    // =============================================
    // EMOSI NPC
    // =============================================
    [Header("Emosi NPC")]
    public int maxSalah = 3;
    private int jumlahSalah  = 0;
    private bool sudahMarah  = false;

    // =============================================
    // PRIVATE STATE
    // =============================================
    private bool sedangKeluar     = false;
    private bool dialogSudahMuncul = false;

    private GameManager gameManager;
    private BubbleChatForcer bubbleForcer;
    private bool kuponDiterima = false;   // NEW
    private KuponPanelUI kuponPanelUI;    // NEW
    private Animator anim;

    // =============================================
    // UNITY LIFECYCLE
    // =============================================
    void Awake()
    {
        anim        = GetComponent<Animator>();
        gameManager = Object.FindFirstObjectByType<GameManager>();
        kuponPanelUI = Object.FindFirstObjectByType<KuponPanelUI>(FindObjectsInactive.Include);
        Debug.Log($"[NPC] kuponPanelUI ditemukan: {kuponPanelUI != null}");  // TEMP

        if (bubbleChatObject != null)
        {
            bubbleForcer = bubbleChatObject.GetComponent<BubbleChatForcer>();
            if (bubbleForcer == null)
                bubbleForcer = bubbleChatObject.AddComponent<BubbleChatForcer>();
        }
    }

    // =============================================
    // SETUP (Dipanggil NPCQueue)
    // =============================================
    public void SetKebutuhan(KebutuhanSet data)
    {
        kebutuhan = data;
        if (logistikText != null) logistikText.text = kebutuhan.logistik.ToString();
        if (firstAidText  != null) firstAidText.text  = kebutuhan.firstAid.ToString();
    }

    public void SetVisual(Sprite img)
    {
        if (avatarImage != null) avatarImage.sprite = img;
    }

    public void SetKupon(KuponInfo data) => kupon = data;      // NEW
    public bool SudahDiterimaKupon() => kuponDiterima;          // NEW

    // Dibiarkan agar tidak ada error compile di script lain yang memanggil ini
    public void SetTargetPos(Vector2 pos) { }
    public Vector2 GetTargetPos() => Vector2.zero;
    public bool SudahTriggerDialog() => dialogSudahMuncul;

    // =============================================
    // ANIMATION EVENT — pasang di akhir clip NPC_IN
    // =============================================

    /// <summary>
    /// Dipanggil oleh Animation Event di akhir clip NPC_IN,
    /// tepat saat NPC sudah berhenti di tengah layar.
    /// Cara pasang: Animation window → clip NPC_IN → frame terakhir → Add Event → OnArrivedAtService
    /// </summary>
    public void OnArrivedAtService()
    {
        if (dialogSudahMuncul || sedangKeluar) return;
        dialogSudahMuncul = true;

        Debug.Log("<color=lime>[NPC]</color> Animation Event: NPC tiba di titik layanan, memicu dialog.");
        StartCoroutine(MunculkanDialog());
    }

    // Method ini tetap ada untuk kompatibilitas dengan NPCQueue lama
    public void TriggerDialogFromQueue() => OnArrivedAtService();

    // =============================================
    // LOGIKA INTERAKSI
    // =============================================
    public void TerimaItem(int l, int f)
    {
        if (sedangKeluar || !kuponDiterima) return; // NEW: cegah servis sebelum kupon di-acc
        if (CekTerpenuhi(l, f)) TriggerKeluar();
        else Salah();
    }

    public bool CekTerpenuhi(int l, int f)
        => l == kebutuhan.logistik && f == kebutuhan.firstAid;

    void Salah()
    {
        jumlahSalah++;
        Debug.Log($"<color=red>[NPC]</color> Salah! ({jumlahSalah}/{maxSalah})");

        if (jumlahSalah >= maxSalah && !sudahMarah) { Marah(); return; }

        TampilkanBubble($"Itu bukan yang saya butuhkan!\n(Peringatan {jumlahSalah}/{maxSalah})");
    }

    public void WrongResponse() => Salah();

    void Marah()
    {
        sudahMarah = true;
        TampilkanBubble("Saya tidak mau dilayani lagi!");
        if (gameManager != null) gameManager.NPCMarah();
        StartCoroutine(TriggerKeluarSetelahDelay(1.5f));
    }

    IEnumerator TriggerKeluarSetelahDelay(float detik)
    {
        yield return new WaitForSeconds(detik);
        TriggerKeluar();
    }

    // =============================================
    // BUBBLE CHAT
    // =============================================
    void TampilkanBubble(string teks)
    {
        if (bubbleChatText != null) bubbleChatText.text = teks;
        if (bubbleForcer   != null) bubbleForcer.Tampilkan();
        else if (bubbleChatObject != null) bubbleChatObject.SetActive(true);
    }

    IEnumerator MunculkanDialog()
    {
        yield return new WaitForSeconds(3f); // Tunggu 1 detik setelah animasi selesai

        if (daftarDialog == null || bubbleChatText == null)
        {
            Debug.LogWarning("[NPC] daftarDialog atau bubbleChatText belum di-assign di prefab!");
            yield break;
        }

        string[] pilihanDialog = null;

        if (kebutuhan.logistik > 0 && kebutuhan.firstAid > 0)
            pilihanDialog = daftarDialog.dialogKeduanya;
        else if (kebutuhan.logistik > 0)
            pilihanDialog = daftarDialog.dialogLogistik;
        else if (kebutuhan.firstAid > 0)
            pilihanDialog = daftarDialog.dialogFirstAid;

        if (pilihanDialog == null || pilihanDialog.Length == 0)
        {
            Debug.LogWarning("[NPC] Array dialog kosong untuk kebutuhan ini.");
            yield break;
        }

        string mentah = pilihanDialog[Random.Range(0, pilihanDialog.Length)];
        string dialogFinal;

        if (kebutuhan.logistik > 0 && kebutuhan.firstAid > 0)
            dialogFinal = string.Format(mentah, kebutuhan.logistik, kebutuhan.firstAid);
        else if (kebutuhan.logistik > 0)
            dialogFinal = string.Format(mentah, kebutuhan.logistik);
        else
            dialogFinal = string.Format(mentah, kebutuhan.firstAid);

        TampilkanBubble(dialogFinal);
        kuponPanelUI?.Tampilkan(this, kupon);
    }
    
    public void OnKeputusanKupon(bool diterima)
{
    if (sedangKeluar) return;

    bool benar = (diterima == kupon.asli);

    if (GameDataManager.Instance != null)
    {
        if (benar) GameDataManager.Instance.kuponBenarHariIni++;
        else       GameDataManager.Instance.kuponSalahHariIni++;
    }

    if (diterima)
    {
        kuponDiterima = true; // buka akses servis logistik di PlayerServe
    }
    else
    {
        TampilkanBubble(kupon.asli
            ? "Tapi kupon saya asli! Kenapa ditolak?"
            : "Baik, saya mengerti.");
        StartCoroutine(TriggerKeluarSetelahDelay(1.2f));
    }
}

    // =============================================
    // EXIT
    // =============================================
    public void TriggerKeluar()
    {
        if (sedangKeluar) return;
        sedangKeluar = true;

        if (bubbleForcer != null) bubbleForcer.Sembunyikan();

        if (anim != null) anim.SetTrigger("Exit");
        Destroy(gameObject, 2f); // NPC dihancurkan setelah 2 detik (durasi animasi keluar)

        StartCoroutine(NotifikasiKeluar());
    }

    IEnumerator NotifikasiKeluar()
    {
        yield return new WaitForSeconds(0.5f);
        Object.FindFirstObjectByType<NPCQueue>()?.RemoveForntNPC();
    }

    // =============================================
    // UNITY LIFECYCLE EXTENSION
    // =============================================
    void OnDestroy()
    {
        // Supaya tidak memicu laporan harian saat ganti scene / keluar game secara paksa
        if (!gameObject.scene.isLoaded) return;

        NPCQueue queue = Object.FindFirstObjectByType<NPCQueue>();
        if (queue != null)
        {
            // Mengecek apakah NPC ini adalah penutup dari barisan shift hari ini
            if (queue.CekShiftSelesai())
            {
                Debug.Log("<color=orange>[NPC]</color> NPC terakhir telah sepenuhnya keluar & hancur. Membuka laporan harian.");
                Object.FindFirstObjectByType<GameManager>()?.NPCFinishedTurn();
            }
        }
    }
}