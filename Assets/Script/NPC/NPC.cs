using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NPC : MonoBehaviour
{
    [Header("Data & Dialog")]
    public KebutuhanSet kebutuhan;
    public DialogData daftarDialog;

    [Header("UI References")]
    public GameObject bubbleChatObject; 
    public TextMeshProUGUI bubbleChatText;
    public Image avatarImage;
    public TextMeshProUGUI logistikText; 
    public TextMeshProUGUI firstAidText; 

    [Header("Settings")]
    public float moveSpeed = 8f;
    [Tooltip("Toleransi jarak untuk memicu dialog. Gunakan 30-50 untuk UI.")]
    public float arrivalThreshold = 40f; 

    private RectTransform rect;
    private Vector2 targetPos;
    private Animator anim;
    private bool sedangKeluar = false;
    private bool dialogSudahMuncul = false;

    private GameManager gameManager;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rect = GetComponent<RectTransform>();
        targetPos = rect.anchoredPosition;

        // Pastikan bubble chat mati saat awal muncul
        if (bubbleChatObject != null) bubbleChatObject.SetActive(false);

        // Cari GameManager di scene (opsional, bisa juga di-assign via Inspector)
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (!sedangKeluar)
        {
            // Pergerakan halus ke posisi target (antrean)
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, Time.deltaTime * moveSpeed);

            // Hitung jarak ke target
            float jarak = Vector2.Distance(rect.anchoredPosition, targetPos);

            // Jika sudah dekat dengan target dan belum pernah ngomong
            if (jarak < arrivalThreshold && !dialogSudahMuncul)
            {
                dialogSudahMuncul = true;
                rect.anchoredPosition = targetPos; 
                StartCoroutine(MunculkanDialog());
            }
        }
    }

    // --- FUNGSI PENGATURAN DATA ---

    public void SetKebutuhan(KebutuhanSet data)
    {
        kebutuhan = data;
        if (logistikText != null) logistikText.text = kebutuhan.logistik.ToString();
        if (firstAidText != null) firstAidText.text = kebutuhan.firstAid.ToString();
        Debug.Log($"<color=yellow>[NPC]</color> Kebutuhan diatur: L={kebutuhan.logistik}, F={kebutuhan.firstAid}");
    }

    public void SetVisual(Sprite img) 
    { 
        if (avatarImage != null) avatarImage.sprite = img; 
    }

    public void SetTargetPos(Vector2 pos) 
    { 
        targetPos = pos; 
    }

    // --- FUNGSI LOGIKA GAME ---

    public bool CekTerpenuhi(int l, int f) 
    { 
        return l >= kebutuhan.logistik && f >= kebutuhan.firstAid; 
    }

    // --- FUNGSI DIALOG ---

    IEnumerator MunculkanDialog()
    {
        yield return new WaitForSeconds(0.25f);
        
        if (bubbleChatObject != null && daftarDialog != null && bubbleChatText != null)
        {
            bubbleChatObject.SetActive(true);
            
            string teksHasil = "";
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
                    teksHasil = string.Format(mentah, kebutuhan.logistik, kebutuhan.firstAid);
                else if (kebutuhan.logistik > 0)
                    teksHasil = string.Format(mentah, kebutuhan.logistik);
                else
                    teksHasil = string.Format(mentah, kebutuhan.firstAid);
            }

            bubbleChatText.text = teksHasil;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(bubbleChatObject.GetComponent<RectTransform>());
            
            Debug.Log($"<color=green>[NPC Success]</color> Dialog muncul di {gameObject.name}: {teksHasil}");
        }
        else
        {
            Debug.LogWarning($"<color=red>[NPC Error]</color> Komponen UI atau DialogData pada {gameObject.name} belum lengkap!");
        }
    }

    // --- FUNGSI KELUAR ---

    public void TriggerKeluar()
    {
        if (sedangKeluar) return;
        sedangKeluar = true;

        if (bubbleChatObject != null) bubbleChatObject.SetActive(false); 
        if (anim != null) anim.SetTrigger("Exit");

        Debug.Log($"<color=magenta>[NPC]</color> {gameObject.name} sedang keluar.");

        // Bisa panggil GameManager jika perlu notifikasi NPC selesai

        Destroy(gameObject, 4f);
    }
}