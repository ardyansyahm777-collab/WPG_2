using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct CharacterSlotDisplay
{
    [Tooltip("UI Image slot karakter (kalau punya komponen AdvancedUIAnimation, masuk/keluarnya otomatis dianimasikan).")]
    public Image slotUI;

    [Tooltip("Sprite karakter yang tampil di slot ini untuk baris dialog ini.")]
    public Sprite sprite;

    [Tooltip("Centang supaya slot ini TAMPIL. Kalau tidak dicentang, slot ini disembunyikan (dengan animasi PlayOut kalau ada).")]
    public bool tampil;

    [Tooltip("Centang kalau karakter di slot ini yang SEDANG BICARA di baris ini (dipakai buat efek dim - hanya berlaku kalau ada 2+ karakter tampil bersamaan).")]
    public bool sedangBicara;
}

[System.Serializable]
public struct DialogChoice
{
    [TextArea(1, 3)] public string teksPilihan;
    [Tooltip("Index baris (di array Cerita Tutorial) yang dituju kalau pilihan ini diklik.")]
    public int tujuanIndex;
}

[System.Serializable]
public struct DialogLine
{
    public string namaKarakter;
    [TextArea(3, 5)] public string isiDialog;

    [Header("Visual Settings - Karakter Utama")]
    public Sprite spriteKarakter;
    public Sprite spritePetunjuk;
    public bool aktifkanPetunjuk;
    [Tooltip("Centang kalau karakter UTAMA yang sedang bicara di baris ini (dipakai buat efek dim saat multi-karakter).")]
    public bool karakterUtamaSedangBicara;

    [Header("Background (opsional)")]
    [Tooltip("Kosongkan kalau background tidak berubah di baris ini.")]
    public Sprite spriteBackground;

    [Header("Multi-Karakter (opsional - isi kalau butuh 2+ karakter tampil bersamaan)")]
    public CharacterSlotDisplay[] karakterTambahan;

    [Header("Aktifkan/Nonaktifkan GameObject Lain (opsional)")]
    [Tooltip("GameObject apapun (gambar, props, panel, dll). Kalau punya AdvancedUIAnimation, otomatis pakai PlayIn().")]
    public GameObject[] objekUntukDiaktifkan;
    [Tooltip("Kalau punya AdvancedUIAnimation, otomatis PlayOut() dulu baru di-nonaktifkan setelah animasinya selesai.")]
    public GameObject[] objekUntukDinonaktifkan;

    [Header("Audio (opsional)")]
    [Tooltip("Ganti BGM kalau diisi dan beda dari yang sedang main.")]
    public AudioClip bgmBaris;
    [Tooltip("SFX sekali putar saat baris ini muncul (mis. suara pintu, ledakan, dll).")]
    public AudioClip sfxBaris;

    [Header("Pilihan/Branching (opsional)")]
    [Tooltip("Kosongkan untuk baris dialog linear biasa. Isi untuk menampilkan tombol pilihan setelah teks selesai diketik.")]
    public DialogChoice[] pilihan;
}

/// <summary>
/// Visual Novel Manager untuk scene cerita (bukan gameplay counter). Mendukung:
/// - Ganti background per baris dialog
/// - Karakter utama + N karakter tambahan tampil bersamaan (multi-slot)
/// - Aktif/nonaktifkan GameObject bebas (gambar, props, dll) per baris
/// - Semua transisi tampil/sembunyi OTOMATIS pakai AdvancedUIAnimation kalau
///   GameObject-nya punya komponen itu (PlayIn saat muncul, PlayOut saat
///   disembunyikan). Kalau tidak punya, fallback ke SetActive polos.
///
/// PENTING: biarkan "Play On Enable" di AdvancedUIAnimation OBJEK-OBJEK ini
/// dalam keadaan TIDAK DICENTANG. VisualNovelManager yang manggil PlayIn()/
/// PlayOut() secara manual sesuai baris dialog - kalau playOnEnable ikut aktif,
/// animasinya bisa kepanggil dobel.
/// </summary>
public class VisualNovelManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtNama; 
    public TextMeshProUGUI txtDialog;
    public Image imgKarakter;        
    public Image imgPetunjukTutor;
    [Tooltip("Opsional - Image background utama scene VN.")]
    public Image imgBackground;

    [Header("Dimming Multi-Karakter (opsional)")]
    [Tooltip("Warna karakter yang SEDANG bicara.")]
    public Color warnaBicara = Color.white;
    [Tooltip("Warna karakter yang TIDAK sedang bicara (redup).")]
    public Color warnaDiam = new Color(0.55f, 0.55f, 0.55f, 1f);

    [Header("Sistem Pilihan/Branching (opsional)")]
    [Tooltip("Panel pembungkus tombol-tombol pilihan. Disembunyikan otomatis kalau baris tidak punya pilihan.")]
    public GameObject panelPilihan;
    [Tooltip("Tombol pilihan yang sudah disiapkan di scene, urut index 0..N. Yang tidak dipakai di suatu baris otomatis disembunyikan.")]
    public Button[] tombolPilihan;
    [Tooltip("Text label untuk tiap tombol di atas (urutan harus sama).")]
    public TextMeshProUGUI[] txtTombolPilihan;

    [Header("Story Settings")]
    public DialogLine[] ceritaTutorial;
    public float typingSpeed = 0.04f;
    public string nextSceneName = "CutScene";
    public float delaySetelahMengetik = 1.0f;

    private int indexDialog = 0; 
    private bool sedangMengetik = false;
    private bool bolehKlikNext = true;
    private bool sedangMenungguPilihan = false;
    private string dialogAktifLengkap = "";

    // Melacak "versi" tiap GameObject yang sedang di-PlayOut, supaya kalau baris
    // berikutnya buru-buru mengaktifkan ulang object yang sama sebelum animasi
    // keluarnya selesai, coroutine PlayOut lama yang lama tidak ikut mematikannya.
    private readonly System.Collections.Generic.Dictionary<GameObject, int> versiOperasiObjek
        = new System.Collections.Generic.Dictionary<GameObject, int>();

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
        // Kalau sedang menampilkan tombol pilihan, klik layar/Space tidak boleh
        // memajukan dialog - pemain harus klik salah satu tombol pilihan dulu.
        if (sedangMenungguPilihan) return;

        if (sedangMengetik)
        {
            // Jika layar diklik saat teks berjalan, matikan Coroutine dan langsung munculkan teks utuh
            StopAllCoroutines();
            txtDialog.text = dialogAktifLengkap; // Mengisi teks secara instan
            SelesaiKetik();
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

    /// <summary>
    /// Dipanggil setiap kali proses pengetikan teks selesai (baik selesai natural
    /// lewat TypeDialog, maupun dipotong manual lewat klik). Kalau baris ini
    /// punya pilihan, tampilkan tombol pilihan alih-alih lanjut ke jeda Next biasa.
    /// </summary>
    private void SelesaiKetik()
    {
        sedangMengetik = false;

        DialogChoice[] pilihanBarisIni = ceritaTutorial[indexDialog].pilihan;
        if (pilihanBarisIni != null && pilihanBarisIni.Length > 0)
        {
            TampilkanPilihan(pilihanBarisIni);
        }
        else
        {
            StartCoroutine(JedaKunciInputNext());
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

        // 2. Background (opsional, ganti sprite langsung kalau diisi)
        if (imgBackground != null && ceritaTutorial[index].spriteBackground != null)
        {
            imgBackground.sprite = ceritaTutorial[index].spriteBackground;
        }

        // 3. Karakter utama - pakai helper slot yang sama supaya animasinya konsisten
        bool karakterUtamaTampil = ceritaTutorial[index].spriteKarakter != null;
        TerapkanSlotKarakter(new CharacterSlotDisplay
        {
            slotUI = imgKarakter,
            sprite = ceritaTutorial[index].spriteKarakter,
            tampil = karakterUtamaTampil
        });

        // 4. Gambar petunjuk tutorial (Logistik, Medic, Laporan Harian, dll)
        if (imgPetunjukTutor != null)
        {
            if (ceritaTutorial[index].aktifkanPetunjuk && ceritaTutorial[index].spritePetunjuk != null)
            {
                imgPetunjukTutor.gameObject.SetActive(true);
                imgPetunjukTutor.sprite = ceritaTutorial[index].spritePetunjuk;
            }
            else
            {
                imgPetunjukTutor.gameObject.SetActive(false);
            }
        }

        // 5. Karakter TAMBAHAN (buat scene dengan 2+ karakter tampil bersamaan)
        if (ceritaTutorial[index].karakterTambahan != null)
        {
            foreach (CharacterSlotDisplay slot in ceritaTutorial[index].karakterTambahan)
            {
                TerapkanSlotKarakter(slot);
            }
        }

        // 6. Aktifkan GameObject bebas apapun (gambar, props, panel, dll) di baris ini
        if (ceritaTutorial[index].objekUntukDiaktifkan != null)
        {
            foreach (GameObject obj in ceritaTutorial[index].objekUntukDiaktifkan)
            {
                AktifkanObjek(obj);
            }
        }

        // 7. Nonaktifkan GameObject bebas apapun di baris ini
        if (ceritaTutorial[index].objekUntukDinonaktifkan != null)
        {
            foreach (GameObject obj in ceritaTutorial[index].objekUntukDinonaktifkan)
            {
                NonaktifkanObjek(obj);
            }
        }

        // 8. Dimming - redupkan karakter yang tidak sedang bicara (kalau multi-karakter aktif)
        TerapkanDimming(index);

        // 9. Audio - ganti BGM / mainkan SFX kalau diisi
        TerapkanAudioBaris(ceritaTutorial[index]);

        // Jalankan efek mengetik teks
        StartCoroutine(TypeDialog(dialogAktifLengkap));
    }

    /// <summary>
    /// Redupkan karakter yang tidak sedang bicara, TAPI HANYA kalau baris ini
    /// memang scene multi-karakter (ada karakterTambahan yang tampil). Untuk
    /// baris dialog karakter tunggal (mode lama), warna dibiarkan normal supaya
    /// data yang sudah diisi sebelumnya tidak berubah perilaku.
    /// </summary>
    private void TerapkanDimming(int index)
    {
        CharacterSlotDisplay[] tambahan = ceritaTutorial[index].karakterTambahan;
        bool adaKarakterTambahanTampil = false;

        if (tambahan != null)
        {
            foreach (CharacterSlotDisplay slot in tambahan)
            {
                if (slot.tampil) { adaKarakterTambahanTampil = true; break; }
            }
        }

        if (!adaKarakterTambahanTampil)
        {
            // Scene karakter tunggal - jangan ganggu warnanya
            if (imgKarakter != null) imgKarakter.color = warnaBicara;
            return;
        }

        if (imgKarakter != null && imgKarakter.gameObject.activeSelf)
        {
            imgKarakter.color = ceritaTutorial[index].karakterUtamaSedangBicara ? warnaBicara : warnaDiam;
        }

        foreach (CharacterSlotDisplay slot in tambahan)
        {
            if (slot.slotUI != null && slot.slotUI.gameObject.activeSelf)
            {
                slot.slotUI.color = slot.sedangBicara ? warnaBicara : warnaDiam;
            }
        }
    }

    private void TerapkanAudioBaris(DialogLine baris)
    {
        if (AudioManager.Instance == null) return;

        if (baris.bgmBaris != null && AudioManager.Instance.bgmSource != null
            && AudioManager.Instance.bgmSource.clip != baris.bgmBaris)
        {
            AudioManager.Instance.bgmSource.clip = baris.bgmBaris;
            AudioManager.Instance.bgmSource.loop = true;
            AudioManager.Instance.bgmSource.Play();
        }

        if (baris.sfxBaris != null)
        {
            AudioManager.Instance.PlaySFXRandomPitch(baris.sfxBaris, 1f, 1f, 1f);
        }
    }

    /// <summary>
    /// Tampilkan tombol-tombol pilihan setelah teks selesai diketik. Player harus
    /// klik salah satu - klik layar/Space tidak akan memajukan dialog selama ini.
    /// </summary>
    private void TampilkanPilihan(DialogChoice[] pilihanBarisIni)
    {
        sedangMenungguPilihan = true;
        bolehKlikNext = false;

        if (panelPilihan != null) panelPilihan.SetActive(true);

        if (tombolPilihan == null) return;

        for (int i = 0; i < tombolPilihan.Length; i++)
        {
            if (tombolPilihan[i] == null) continue;

            if (i < pilihanBarisIni.Length)
            {
                tombolPilihan[i].gameObject.SetActive(true);

                if (txtTombolPilihan != null && i < txtTombolPilihan.Length && txtTombolPilihan[i] != null)
                {
                    txtTombolPilihan[i].text = pilihanBarisIni[i].teksPilihan;
                }

                int tujuanIndex = pilihanBarisIni[i].tujuanIndex; // capture lokal untuk closure
                tombolPilihan[i].onClick.RemoveAllListeners();
                tombolPilihan[i].onClick.AddListener(() => PilihOpsi(tujuanIndex));
            }
            else
            {
                tombolPilihan[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// PANGGIL FUNGSI INI DARI ONCLICK TOMBOL PILIHAN (sudah di-set otomatis
    /// lewat TampilkanPilihan, tapi bisa juga di-wire manual kalau perlu).
    /// </summary>
    public void PilihOpsi(int tujuanIndex)
    {
        if (panelPilihan != null) panelPilihan.SetActive(false);
        sedangMenungguPilihan = false;

        if (ceritaTutorial != null && tujuanIndex >= 0 && tujuanIndex < ceritaTutorial.Length)
        {
            indexDialog = tujuanIndex;
            TampilkanLine(indexDialog);
        }
        else
        {
            SelesaiTutorial();
        }
    }

    /// <summary>
    /// Terapkan 1 slot karakter (utama maupun tambahan). Kalau slot BARU muncul
    /// (sebelumnya tersembunyi), animasikan masuk via AdvancedUIAnimation kalau
    /// ada. Kalau slot sudah tampil dari baris sebelumnya, cuma ganti sprite
    /// tanpa mengulang animasi masuk (karakter yang sudah di layar tidak perlu
    /// "masuk" lagi tiap ganti baris dialog).
    /// </summary>
    private void TerapkanSlotKarakter(CharacterSlotDisplay slot)
    {
        if (slot.slotUI == null) return;

        bool sedangTampil = slot.slotUI.gameObject.activeSelf;

        if (slot.tampil && slot.sprite != null)
        {
            slot.slotUI.sprite = slot.sprite;

            if (!sedangTampil)
            {
                AktifkanObjek(slot.slotUI.gameObject);
            }
        }
        else if (sedangTampil)
        {
            NonaktifkanObjek(slot.slotUI.gameObject);
        }
    }

    /// <summary>
    /// Aktifkan GameObject. Kalau punya AdvancedUIAnimation, panggil PlayIn()
    /// supaya animasi masuknya jalan (slide/fade/pop sesuai konfigurasi).
    /// </summary>
    private void AktifkanObjek(GameObject obj)
    {
        if (obj == null) return;

        // Naikkan versi operasi object ini - kalau ada coroutine PlayOut lama
        // yang masih menunggu, versinya jadi tidak cocok lagi dan dia tidak
        // akan menonaktifkan object yang baru saja diaktifkan ulang ini.
        versiOperasiObjek[obj] = GetVersiOperasi(obj) + 1;

        obj.SetActive(true);

        AdvancedUIAnimation anim = obj.GetComponent<AdvancedUIAnimation>();
        if (anim != null) anim.PlayIn();
    }

    /// <summary>
    /// Nonaktifkan GameObject. Kalau punya AdvancedUIAnimation, mainkan PlayOut()
    /// dulu dan tunggu durasinya selesai baru benar-benar di-SetActive(false) -
    /// supaya animasi keluarnya sempat terlihat, bukan langsung hilang mendadak.
    /// </summary>
    private void NonaktifkanObjek(GameObject obj)
    {
        if (obj == null) return;

        AdvancedUIAnimation anim = obj.GetComponent<AdvancedUIAnimation>();
        if (anim != null)
        {
            int versiSaatIni = GetVersiOperasi(obj);
            StartCoroutine(PlayOutLaluNonaktifkan(anim, obj, versiSaatIni));
        }
        else
        {
            obj.SetActive(false);
        }
    }

    private IEnumerator PlayOutLaluNonaktifkan(AdvancedUIAnimation anim, GameObject obj, int versiSaatDipanggil)
    {
        anim.PlayOut();
        yield return new WaitForSeconds(anim.duration);

        // Kalau di antara PlayOut() dan sekarang, AktifkanObjek() sempat dipanggil
        // lagi untuk object yang sama (baris berikutnya buru-buru memunculkannya
        // lagi), versi operasinya sudah naik - jangan matikan object ini.
        if (obj != null && GetVersiOperasi(obj) == versiSaatDipanggil)
        {
            obj.SetActive(false);
        }
    }

    private int GetVersiOperasi(GameObject obj)
    {
        return versiOperasiObjek.TryGetValue(obj, out int versi) ? versi : 0;
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
        SelesaiKetik();
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