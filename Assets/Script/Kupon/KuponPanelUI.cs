using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KuponPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelKupon;
    public TextMeshProUGUI txtNomor;
    public TextMeshProUGUI txtNama;
    public TextMeshProUGUI txtTanggal;
    public Image imgStempel;
    public Button btnTerima;
    public Button btnTolak;

    private NPC npcAktif;

    void Awake()
    {
        if (panelKupon != null) panelKupon.SetActive(false);
        btnTerima.onClick.AddListener(() => Putuskan(true));
        btnTolak.onClick.AddListener(() => Putuskan(false));
    }

    public void Tampilkan(NPC npc, KuponInfo kupon)
    {
        npcAktif = npc;
        txtNomor.text   = kupon.nomorRegistrasi;
        txtNama.text    = kupon.namaPengungsi;
        txtTanggal.text = kupon.tanggalTerbit;
        imgStempel.sprite = kupon.stempelSprite;
        panelKupon.SetActive(true);
    }

    public void Putuskan(bool diterima)
    {
        panelKupon.SetActive(false);
        npcAktif?.OnKeputusanKupon(diterima);
    }
}