using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RuleAndSPManager : MonoBehaviour
{
    public static RuleAndSPManager Instance { get; private set; }

    [Header("Sistem Surat Peringatan (SP)")]
    public int jumlahSPSaatIni = 0;
    public int maxSP = 3; 

    [Header("UI Display Rulebook & SP")]
    public TextMeshProUGUI txtRulebookHarian;
    public TextMeshProUGUI txtStatusSP;
    public GameObject panelGameOverSP; 

    [Header("Notifikasi Animasi SP (Advanced UI)")]
    [Tooltip("Assign GameObject 'peringatan_SP' dari Hierarchy yang memiliki skrip AdvancedUIAnimation")]
    public GameObject panelNotifikasiSP;

    [Tooltip("Durasi notifikasi SP tampil di layar sebelum turun kembali (detik)")]
    public float durasiTampilSP = 3.0f;

    private Coroutine routineNotifSP;

    private Dictionary<int, string> daftarAturanHarian = new Dictionary<int, string>()
    {
        { 1, "Periksa nama yang ada didalam dengan cermat" },
        { 2, "DILARANG meloloskan warga luar Aceh tanpa stempel Posko Pusat." },
        { 3, "DILARANG menerima Kupon/Kertas Hitam dengan stempel pudar/palsu." },
        { 4, "DILARANG memberikan bantuan jika Foto KTP tidak cocok dengan warga." },
        { 5, "CRISIS! DILARANG memberikan Paket Medis tanpa Kupon/Voucher Medis." },
        { 6, "BADAI SUSULAN! DILARANG meloloskan dokumen yang memiliki Typo Nama/NIK." },
        { 7, "DARURAT! Bantuan HANYA untuk warga lokal yang mengalami cedera/membawa dokumen sah." }
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelGameOverSP != null) panelGameOverSP.SetActive(false);
        if (panelNotifikasiSP != null) panelNotifikasiSP.SetActive(false);
        UpdateSPUI();
    }

    public void PerbaruiRulebookHarian(int hariSekarang)
    {
        if (txtRulebookHarian == null) return;

        string teksRulebook = ""; 

        for (int i = 1; i <= hariSekarang; i++)
        {
            if (daftarAturanHarian.ContainsKey(i))
            {
                teksRulebook += daftarAturanHarian[i] + "\n";
            }
        }

        txtRulebookHarian.text = teksRulebook;
    }

    public void TambahSP(int jumlah)
    {
        jumlahSPSaatIni += jumlah;
        Debug.LogWarning($"[SP Manager] Player menerima {jumlah} SP! Total SP: {jumlahSPSaatIni}/{maxSP}");

        UpdateSPUI();

        // Tampilkan animasi notifikasi SP naik -> tunggu 3 detik -> turun
        if (routineNotifSP != null) StopCoroutine(routineNotifSP);
        routineNotifSP = StartCoroutine(RoutineAnimasiNotifSP());

        if (jumlahSPSaatIni >= maxSP)
        {
            Invoke(nameof(TriggerGameOverPecat), 1.5f);
        }
    }

    /// <summary>
    /// Coroutine untuk memunculkan notifikasi SP (PlayIn), menahannya selama 3 detik,
    /// lalu menurunkannya kembali (PlayOut).
    /// </summary>
    private IEnumerator RoutineAnimasiNotifSP()
    {
        if (panelNotifikasiSP == null) yield break;

        panelNotifikasiSP.SetActive(true);
        AdvancedUIAnimation anim = panelNotifikasiSP.GetComponent<AdvancedUIAnimation>();

        if (anim != null)
        {
            // 1. Naikkan / Munculkan Notifikasi
            anim.PlayIn(); 

            // 2. Tunggu selama durasi animasi masuk + jeda 3 detik
            yield return new WaitForSeconds(anim.duration + durasiTampilSP);

            // 3. Turunkan / Sembunyikan Notifikasi
            anim.PlayOut();

            // 4. Tunggu hingga animasi keluar selesai, baru nonaktifkan GameObject
            yield return new WaitForSeconds(anim.duration);
        }
        else
        {
            yield return new WaitForSeconds(durasiTampilSP);
        }

        panelNotifikasiSP.SetActive(false);
        routineNotifSP = null;
    }

    private void UpdateSPUI()
    {
        if (txtStatusSP != null)
            txtStatusSP.text = $"Surat Peringatan: {jumlahSPSaatIni} / {maxSP}";
    }

    private void TriggerGameOverPecat()
    {
        Time.timeScale = 0f;

        if (panelGameOverSP != null)
        {
            panelGameOverSP.SetActive(true);
        }
        else
        {
            GameManager gm = Object.FindFirstObjectByType<GameManager>();
            if (gm != null && gm.losePanel != null)
            {
                if (gm.txtAlasanKalah != null)
                    gm.txtAlasanKalah.text = "KAMU DIPECAT!\nKamu menerima 3 Surat Peringatan (SP) akibat terlalu banyak melanggar prosedur posko.";
                
                gm.losePanel.SetActive(true);
            }
        }
    }
}