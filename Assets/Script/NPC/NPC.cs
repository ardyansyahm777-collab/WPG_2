using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic; // Wajib ditambahkan untuk List

public class NPC : MonoBehaviour
{
    [Header("Tipe NPC")]
    public bool isStoryNPC = false;

    [Header("Data & Dialog")]
    public KebutuhanSet kebutuhan;
    public DialogData daftarDialog;
    public KuponInfo kupon;
    public List<VoucherInfo> daftarVoucher = new List<VoucherInfo>();
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
            avatarImage.SetNativeSize(); 
            avatarImage.preserveAspect = true; 
        }
    }

    public void SetKupon(KuponInfo data)
    {
        if (!isStoryNPC) kupon = data;
    }

    // Fungsi baru untuk menerima daftar voucher
    public void SetVouchers(List<VoucherInfo> dataList)
    {
        if (!isStoryNPC) daftarVoucher = dataList;
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

            // Tampilkan dokumen lainnya
            ktpPanelUI?.Tampilkan(ktp);
            kuponPanelUI?.Tampilkan(this, kupon);
            
            // PASTIKAN BARIS INI MEMANGGIL DAFTAR VOUCHER
            if (voucherPanelUI != null)
            {
                voucherPanelUI.TampilkanSemua(daftarVoucher);
            }
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

        if (GameDataManager.Instance == null) return;

        // KASUS 1: NPC Membawa Kupon Biasa (Non-Story)
        if (kupon != null)
        {
            bool keputusanBenar = (diterima == kupon.asli);

            if (keputusanBenar)
            {
                GameDataManager.Instance.kuponBenarHariIni++;

                if (diterima)
                {
                    // Meloloskan dokumen sah
                    GameDataManager.Instance.TambahMetrik(compliance: 1, humanity: 1);
                }
                else
                {
                    // Menolak dokumen palsu/mismatch dengan tepat
                    GameDataManager.Instance.TambahMetrik(compliance: 1, humanity: 0);
                }
            }
            else
            {
                GameDataManager.Instance.kuponSalahHariIni++;

                if (diterima && !kupon.asli)
                {
                    // KASUS: Meloloskan Dokumen Palsu / Mismatch (Pelanggaran SOP)
                    GameDataManager.Instance.TambahMetrik(compliance: -1, humanity: 0);

                    // Tambah SP jika melanggar SOP
                    if (RuleAndSPManager.Instance != null)
                        RuleAndSPManager.Instance.TambahSP(1);
                }
                else if (!diterima && kupon.asli)
                {
                    // KASUS: Menolak Warga Berdokumen Sah
                    GameDataManager.Instance.TambahMetrik(compliance: -1, humanity: -1);
                }
            }
        }
        // KASUS 2: NPC Khusus / Dilema Moral (Misal: Dokumen Robek/Hanyut tapi Butuh Obat Urgent)
        else if (isStoryNPC)
        {
            if (diterima)
            {
                // Pemain memilih Kemanusiaan (Melanggar aturan demi menolong)
                GameDataManager.Instance.TambahMetrik(compliance: -1, humanity: 2);
            }
            else
            {
                // Pemain memilih Kepatuhan Birokrasi Keras
                GameDataManager.Instance.TambahMetrik(compliance: 2, humanity: -1);
            }
        }

        if (diterima)
        {
            kuponDiterima = true;
            GameDataManager.Instance.wargaDibantu++;
            GameDataManager.Instance.totalWargaDibantu++;
        }

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