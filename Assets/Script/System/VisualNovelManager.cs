using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct DialogLine
{
    public string namaKarakter;
    [TextArea(3, 5)] public string isiDialog;
    
    [Header("Visual Settings")]
    public Sprite spriteKarakter;    
    public Sprite spritePetunjuk;    
    public bool aktifkanPetunjuk;    
}

public class VisualNovelManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtNama; 
    public TextMeshProUGUI txtDialog;
    public Image imgKarakter;        
    public Image imgPetunjukTutor; 

    [Header("Story Settings")]
    public DialogLine[] ceritaTutorial;
    public float typingSpeed = 0.04f;
    public string nextSceneName = "CutScene";
    public float delaySetelahMengetik = 1.0f;

    private int indexDialog = 0; 
    private bool sedangMengetik = false;
    private bool bolehKlikNext = true; 
    private string dialogAktifLengkap = "";

    void Start()
    {
        indexDialog = 0; // Reset index di awal scene
        bolehKlikNext = true;
        
        // Menyembunyikan komponen UI petunjuk di awal dialog agar tidak mengganggu
        if (imgPetunjukTutor != null) 
            imgPetunjukTutor.gameObject.SetActive(false);

        MulaiDialog(); // Memulai alur VN
    }

    void Update()
    {
        // Tombol Space pada keyboard tetap dipertahankan sebagai alternatif kontrol
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProsesKemajuanDialog(); // Maju atau selesaikan ketikan teks
        }
    }

    /// <summary>
    /// PANGGIL FUNGSI INI MELALUI KOMPONEN BUTTON ATAU EVENT TRIGGER DI PANEL DIALOG (UI SCREEN CLICK)
    /// </summary>
    public void OnPointerClickDialog()
    {
        ProsesKemajuanDialog(); // Memproses kemajuan VN saat area layar diklik
    }

    private void ProsesKemajuanDialog()
    {
        if (sedangMengetik)
        {
            // Jika layar diklik saat teks berjalan, matikan Coroutine dan langsung munculkan teks utuh
            StopAllCoroutines();
            txtDialog.text = dialogAktifLengkap; // Mengisi teks secara instan
            sedangMengetik = false; // Mengubah status mengetik menjadi selesai
            
            // Karena dipotong manual, kita langsung jalankan coroutine pengunci delay di sini
            StartCoroutine(JedaKunciInputNext());
        }
        else
        {
            // Jika teks sudah berhenti mengetik, pastikan dulu status kunci delay sudah terbuka (bolehKlikNext == true)
            if (bolehKlikNext)
            {
                NextLine();
            }
        }
    }

    void MulaiDialog()
    {
        // Validasi jika data array cerita kosong agar game tidak mengalami error
        if (ceritaTutorial == null || ceritaTutorial.Length == 0)
        {
            SelesaiTutorial(); // Langsung lompat keluar scene
            return;
        }
        TampilkanLine(indexDialog); // Tampilkan baris pertama
    }

    void TampilkanLine(int index)
    {
        // Kunci input tombol Next di awal baris baru
        bolehKlikNext = false;

        // 1. Perbarui teks nama dan simpan teks lengkap dialog
        txtNama.text = ceritaTutorial[index].namaKarakter;
        dialogAktifLengkap = ceritaTutorial[index].isiDialog;

        // 2. Perbarui gambar karakter Hartono jika sprite-nya tersedia
        if (imgKarakter != null)
        {
            if (ceritaTutorial[index].spriteKarakter != null)
            {
                imgKarakter.gameObject.SetActive(true);
                imgKarakter.sprite = ceritaTutorial[index].spriteKarakter; // Mengganti isi UI Image dengan Sprite cerita
            }
            else
            {
                imgKarakter.gameObject.SetActive(false);
            }
        }

        // 3. Perbarui gambar petunjuk visual tutorial (Logistik, Medic, atau Laporan Harian)
        if (imgPetunjukTutor != null)
        {
            if (ceritaTutorial[index].aktifkanPetunjuk && ceritaTutorial[index].spritePetunjuk != null)
            {
                imgPetunjukTutor.gameObject.SetActive(true); // Memunculkan UI Image petunjuk
                imgPetunjukTutor.sprite = ceritaTutorial[index].spritePetunjuk; // Memasukkan gambar petunjuk yang sesuai
            }
            else
            {
                imgPetunjukTutor.gameObject.SetActive(false); // Sembunyikan jika tidak diaktifkan
            }
        }

        // Jalankan efek mengetik teks
        StartCoroutine(TypeDialog(dialogAktifLengkap));
    }

IEnumerator TypeDialog(string dialog)
    {
        sedangMengetik = true;
        txtDialog.text = "";
        
        int hitungHuruf = 0; // Variabel penanda hitungan huruf

        foreach (char huruf in dialog)
        {
            txtDialog.text += huruf;

            // --- SISTEM PEMBATAS SUARA KETIK ---
            // Hanya bunyikan SFX jika hitungan huruf kelipatan 2 (bisa kamu ganti ke 3 jika masih kemurahan)
            if (hitungHuruf % 2 == 0) 
            {
                if (AudioManager.Instance != null && AudioManager.Instance.vnClick != null)
                {
                    AudioManager.Instance.PlaySFXRandomPitch(AudioManager.Instance.vnClick, 0.7f, 0.85f, 1.15f);
                }
            }
            hitungHuruf++; // Naikkan hitungan huruf
            // ------------------------------------

            yield return new WaitForSeconds(typingSpeed);
        }
        sedangMengetik = false;
        StartCoroutine(JedaKunciInputNext());
    }

    /// <summary>
    /// Coroutine untuk memberikan jeda aman sebelum tombol Next Line bisa merespons klik berikutnya
    /// </summary>
    IEnumerator JedaKunciInputNext()
    {
        bolehKlikNext = false; // Pastikan input terkunci
        yield return new WaitForSeconds(delaySetelahMengetik); // Menunggu selama 1 detik (sesuai variabel global)
        bolehKlikNext = true; // Membuka kembali izin akses klik Next Line
    }

    public void NextLine()
    {
        indexDialog++; // Naikkan index ke baris berikutnya
        if (indexDialog < ceritaTutorial.Length)
        {
            TampilkanLine(indexDialog); // Tampilkan baris baru jika masih ada sisa data
        }
        else
        {
            SelesaiTutorial(); // Akhiri tutorial jika seluruh baris telah habis
        }
    }

    /// <summary>
    /// PANGGIL FUNGSI INI DARI UI BUTTON "SKIP" DI UNITY INSPECTOR
    /// </summary>
    public void TombolSkipTutorial()
    {
        StopAllCoroutines(); // Hentikan proses mengetik jika tombol skip ditekan mid-dialog
        SelesaiTutorial();   // Langsung panggil fungsi transisi scene
    }

    public void SelesaiTutorial()
    {
        Debug.Log("[VN System] Tutorial selesai atau di-skip. Melompat ke Day 1.");
        
        
        if (DayTransitionManager.Instance != null)
        {
            DayTransitionManager.Instance.MulaiDariTutorial();
        }
    }
}