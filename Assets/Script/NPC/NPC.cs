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

    void Awake()
    {
        anim = GetComponent<Animator>();
        rect = GetComponent<RectTransform>();
        targetPos = rect.anchoredPosition;
        
        // Pastikan bubble chat mati saat awal muncul
        if (bubbleChatObject != null) bubbleChatObject.SetActive(false);
    }

    void Update()
    {
        if (!sedangKeluar)
        {
            // Pergerakan halus ke posisi target (antrean)
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, Time.deltaTime * moveSpeed);

            // Hitung jarak ke target
            float jarak = Vector2.Distance(rect.anchoredPosition, targetPos);
            
            // Jika sudah sangat dekat dengan target dan belum pernah ngomong
            if (jarak < arrivalThreshold && !dialogSudahMuncul)
            {
                dialogSudahMuncul = true;
                // Paksa posisi ke target agar presisi
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
        // Jika NPC pindah ke titik antrean baru, kita tidak mereset dialogSudahMuncul 
        // agar dia tidak ngomong berkali-kali saat bergeser di antrean.
    }

    // --- FUNGSI LOGIKA GAME ---

    public bool CekTerpenuhi(int l, int f) 
    { 
        return l >= kebutuhan.logistik && f >= kebutuhan.firstAid; 
    }

    // --- FUNGSI DIALOG ---

    IEnumerator MunculkanDialog()
    {
        // Jeda sedikit agar dialog muncul tepat setelah berhenti
        //Bubble chat program
        yield return new WaitForSeconds(0.25f);
        
        if (bubbleChatObject != null && daftarDialog != null && bubbleChatText != null)
        {
            bubbleChatObject.SetActive(true);
            
            string teksHasil = "";
            string[] pilihanDialog = null;

            // Memilih kategori dialog berdasarkan isi kebutuhan
            if (kebutuhan.logistik > 0 && kebutuhan.firstAid > 0)
                pilihanDialog = daftarDialog.dialogKeduanya;
            else if (kebutuhan.logistik > 0)
                pilihanDialog = daftarDialog.dialogLogistik;
            else if (kebutuhan.firstAid > 0)
                pilihanDialog = daftarDialog.dialogFirstAid;

            // Ambil teks acak dan masukkan angka kebutuhan {0} atau {1}
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

            // Refresh Layout agar background (Image) mengikuti panjang teks (TMP)
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
        
        // Sembunyikan bubble saat pergi agar tidak melayang di udara
        if (bubbleChatObject != null) bubbleChatObject.SetActive(false); 
        
        if (anim != null) anim.SetTrigger("Exit");
        
        Debug.Log($"<color=magenta>[NPC]</color> {gameObject.name} sedang keluar.");
        Destroy(gameObject, 4f);
    }
}