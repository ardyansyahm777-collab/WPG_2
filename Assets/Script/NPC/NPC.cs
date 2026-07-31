using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NPC : MonoBehaviour
{
    [Header("Tipe NPC")]
    [Tooltip("Centang jika ini adalah Tokoh Cerita Penting (Teuku Rahman, Nyak Meamunah, dr. Maya, dll)")]
    public bool isStoryNPC = false;

    [Header("Data & Dialog")]
    public KebutuhanSet kebutuhan;
    public DialogData daftarDialog;
    public KuponInfo kupon;
    public VoucherInfo voucher;
    public KTPInfo ktp;

    [Header("UI References")]
    public GameObject bubbleChatObject;
    public TextMeshProUGUI bubbleChatText;
    public Image avatarImage;
    public TextMeshProUGUI logistikText;
    public TextMeshProUGUI firstAidText;

    private bool sedangKeluar = false;
    private bool dialogSudahMuncul = false;

    private GameManager gameManager;
    private BubbleChatForcer bubbleForcer;
    private bool kuponDiterima = false;
    private KuponPanelUI kuponPanelUI;
    private VoucherPanelUI voucherPanelUI;
    private KTPPanelUI ktpPanelUI;
    private Animator anim;

    // NPC keluar setelah keputusan Kupon (Kartu Bantuan) diambil - Voucher & KTP
    // sekarang murni dokumen referensi (tanpa tombol), jadi tidak perlu ditunggu.
    private bool keputusanSudahDiambil = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        gameManager = Object.FindFirstObjectByType<GameManager>();
        kuponPanelUI = Object.FindFirstObjectByType<KuponPanelUI>(FindObjectsInactive.Include);
        voucherPanelUI = Object.FindFirstObjectByType<VoucherPanelUI>(FindObjectsInactive.Include);
        ktpPanelUI = Object.FindFirstObjectByType<KTPPanelUI>(FindObjectsInactive.Include);

        if (bubbleChatObject != null)
        {
            bubbleForcer = bubbleChatObject.GetComponent<BubbleChatForcer>();
            if (bubbleForcer == null)
                bubbleForcer = bubbleChatObject.AddComponent<BubbleChatForcer>();
        }
    }

    // Fungsi Start() dan SetupRandomNPCData() DIHAPUS karena sinkronisasi
    // gambar dan dokumen sekarang di-handle oleh NPCQueue saat Spawn.

    public void SetKebutuhan(KebutuhanSet data)
    {
        kebutuhan = data;
        if (logistikText != null) logistikText.text = kebutuhan.logistik.ToString();
        if (firstAidText != null) firstAidText.text = kebutuhan.firstAid.ToString();
    }

    public void SetVisual(Sprite img)
    {
        if (avatarImage != null && img != null) 
        {
            avatarImage.sprite = img;
            
            // Mengembalikan ukuran UI Image ke piksel asli gambar tanpa penyok
            avatarImage.SetNativeSize(); 

            // Jaga agar opsi preserve aspect tetap aktif
            avatarImage.preserveAspect = true; 
        }
    }

    public void SetKupon(KuponInfo data)
    {
        if (!isStoryNPC) kupon = data;
    }

    public void SetVoucher(VoucherInfo data)
    {
        if (!isStoryNPC) voucher = data;
    }

    public void SetKTP(KTPInfo data)
    {
        if (!isStoryNPC) ktp = data;
    }

    public bool SudahDiterimaKupon() => kuponDiterima;

    public void OnArrivedAtService()
    {
        if (dialogSudahMuncul || sedangKeluar) return;
        dialogSudahMuncul = true;

        StartCoroutine(MunculkanDialog());
    }

    IEnumerator MunculkanDialog()
    {
        yield return new WaitForSeconds(1f);

        if (daftarDialog == null || bubbleChatText == null) yield break;

        string dialogFinal = "";

        if (!string.IsNullOrEmpty(daftarDialog.dialogUtamaSpontan))
        {
            dialogFinal = daftarDialog.dialogUtamaSpontan;
        }
        else
        {
            string[] pilihanDialog = null;

            if (kebutuhan.logistik > 0 && kebutuhan.firstAid > 0)
                pilihanDialog = daftarDialog.dialogKeduanya;
            else if (kebutuhan.logistik > 0)
                pilihanDialog = daftarDialog.dialogLogistik;
            else if (kebutuhan.firstAid > 0)
                pilihanDialog = daftarDialog.dialogFirstAid;

            if (pilihanDialog != null && pilihanDialog.Length > 0)
            {
                string mentah = pilihanDialog[Random.Range(0, pilihanDialog.Length)];

                if (kebutuhan.logistik > 0 && kebutuhan.firstAid > 0)
                    dialogFinal = string.Format(mentah, kebutuhan.logistik, kebutuhan.firstAid);
                else if (kebutuhan.logistik > 0)
                    dialogFinal = string.Format(mentah, kebutuhan.logistik);
                else
                    dialogFinal = string.Format(mentah, kebutuhan.firstAid);
            }
        }

        if (!string.IsNullOrEmpty(dialogFinal))
        {
            TampilkanBubble(dialogFinal);

            // Semua dokumen tampil BERSAMAAN. KTP & Voucher murni referensi
            // (tanpa tombol) - pemain membandingkannya sendiri sebelum
            // memutuskan Terima/Tolak di panel Kupon (Kartu Bantuan).
            ktpPanelUI?.Tampilkan(ktp);
            kuponPanelUI?.Tampilkan(this, kupon);
            voucherPanelUI?.Tampilkan(voucher);
        }
    }

    public void TerimaItem(int l, int f)
    {
        if (sedangKeluar || !kuponDiterima) return;
        if (CekTerpenuhi(l, f)) TriggerKeluar();
    }

    public bool CekTerpenuhi(int l, int f) => l == kebutuhan.logistik && f == kebutuhan.firstAid;

    void TampilkanBubble(string teks)
    {
        if (bubbleChatText != null) bubbleChatText.text = teks;
        if (bubbleForcer != null) bubbleForcer.Tampilkan();
        else if (bubbleChatObject != null) bubbleChatObject.SetActive(true);
    }

    public void OnKeputusanKupon(bool diterima)
    {
        if (sedangKeluar || keputusanSudahDiambil) return;
        keputusanSudahDiambil = true;

        if (kupon != null)
        {
            bool benar = (diterima == kupon.asli);

            if (GameDataManager.Instance != null)
            {
                if (benar) GameDataManager.Instance.kuponBenarHariIni++;
                else GameDataManager.Instance.kuponSalahHariIni++;
            }
        }
        else
        {
            // NPC tanpa dokumen sama sekali (mis. Teuku Rahman, dr. Maya) bukan
            // kasus deteksi pemalsuan biasa - ini dilema compliance vs humanity,
            // jadi dicatat lewat metrik ending, bukan skor benar/salah kupon.
            if (GameDataManager.Instance != null)
            {
                if (diterima) GameDataManager.Instance.TambahMetrik(compliance: -1, humanity: 1);
                else GameDataManager.Instance.TambahMetrik(compliance: 1, humanity: -1);
            }
        }

        if (diterima)
        {
            kuponDiterima = true;
        }

        TampilkanBubble(diterima
            ? "Terima kasih banyak, Mas!"
            : (kupon != null && kupon.asli ? "Tapi kupon saya asli! Kenapa ditolak?" : "Baik, saya mengerti."));

        StartCoroutine(TriggerKeluarSetelahDelay(diterima ? 1.0f : 1.2f));
    }

    IEnumerator TriggerKeluarSetelahDelay(float detik)
    {
        yield return new WaitForSeconds(detik);
        TriggerKeluar();
    }

    public void TriggerKeluar()
    {
        if (sedangKeluar) return;
        sedangKeluar = true;

        if (bubbleForcer != null) bubbleForcer.Sembunyikan();
        ktpPanelUI?.Sembunyikan();
        voucherPanelUI?.Sembunyikan();

        if (anim != null) anim.SetTrigger("Exit");
        Destroy(gameObject, 2f);

        StartCoroutine(NotifikasiKeluar());
    }

    IEnumerator NotifikasiKeluar()
    {
        yield return new WaitForSeconds(0.5f);
        Object.FindFirstObjectByType<NPCQueue>()?.RemoveForntNPC();
    }

    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;

        NPCQueue queue = Object.FindFirstObjectByType<NPCQueue>();
        if (queue != null && queue.CekShiftSelesai())
        {
            Object.FindFirstObjectByType<GameManager>()?.NPCFinishedTurn();
        }
    }
}