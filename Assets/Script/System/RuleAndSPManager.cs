using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RuleAndSPManager : MonoBehaviour
{
    public static RuleAndSPManager Instance { get; private set; }

    [Header("Sistem Surat Peringatan (SP)")]
    public int jumlahSPSaatIni = 0;
    public int maxSP = 3; 

    [Header("UI Display Rulebook")]
    public TextMeshProUGUI txtRulebookHarian;
    public TextMeshProUGUI txtStatusSP;
    public GameObject panelGameOverSP; 

    private Dictionary<int, string> daftarAturanHarian = new Dictionary<int, string>()
    {
        { 1, "• KTP yang kedaluwarsa sebelum 26 Des 2004 TIDAK VALID." },
        { 2, "• DILARANG meloloskan warga luar Aceh tanpa stempel Posko Pusat." },
        { 3, "• DILARANG menerima Kupon/Kertas Hitam dengan stempel pudar." },
        { 4, "• DILARANG memberikan bantuan mewakili orang lain (1 KTP = 1 Orang)." },
        { 5, "• CRISIS! DILARANG memberikan Paket Medis tanpa Kupon Merah." },
        { 6, "• BADAI SUSULAN! DILARANG meloloskan dokumen yang memiliki Typo Nama." },
        { 7, "• DARURAT! Bantuan HANYA untuk warga lokal yang cedera berat." }
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelGameOverSP != null) panelGameOverSP.SetActive(false);
        UpdateSPUI();
    }

    public void PerbaruiRulebookHarian(int hariSekarang)
    {
        if (txtRulebookHarian == null) return;

        string teksRulebook = "<b>=== PERATURAN & LARANGAN HARI INI ===</b>\n\n";

        for (int i = 1; i <= hariSekarang; i++)
        {
            if (daftarAturanHarian.ContainsKey(i))
            {
                teksRulebook += $"<b>[HARI {i}]</b>\n" + daftarAturanHarian[i] + "\n\n";
            }
        }

        txtRulebookHarian.text = teksRulebook;
    }

    public void TambahSP(int jumlah)
    {
        jumlahSPSaatIni += jumlah;
        Debug.LogWarning($"[SP Manager] Player menerima {jumlah} SP! Total SP: {jumlahSPSaatIni}/{maxSP}");

        UpdateSPUI();

        if (jumlahSPSaatIni >= maxSP)
        {
            TriggerGameOverPecat();
        }
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