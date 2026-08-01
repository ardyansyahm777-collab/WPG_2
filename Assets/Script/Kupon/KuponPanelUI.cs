using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KuponPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelKupon;
    public TextMeshProUGUI txtNomorRegistrasi;
    public TextMeshProUGUI txtNama;
    public TextMeshProUGUI txtNik;           // Drag TMP_Text NIK Kupon ke sini
    public TextMeshProUGUI txtTanggalLahir;  // Drag TMP_Text Tanggal Lahir Kupon ke sini
    public TextMeshProUGUI txtTanggalTerbit; 
    public Image imgFotoKupon;
    public Image imgStempel;
    public Button btnTerima;
    public Button btnTolak;

    private NPC npcAktif;

    void Awake()
    {
        if (panelKupon != null) panelKupon.SetActive(false);
        if (btnTerima != null) btnTerima.onClick.AddListener(() => Putuskan(true));
        if (btnTolak != null) btnTolak.onClick.AddListener(() => Putuskan(false));
    }

    public void Tampilkan(NPC npc, KuponInfo kupon)
    {
        if (kupon == null)
        {
            panelKupon.SetActive(false);
            return;
        }

        npcAktif = npc;
        if (txtNomorRegistrasi != null) txtNomorRegistrasi.text = kupon.nomorRegistrasi;
        if (txtNama != null)            txtNama.text            = kupon.namaPengungsi;
        if (txtNik != null)             txtNik.text             = kupon.nik;
        if (txtTanggalLahir != null)    txtTanggalLahir.text    = kupon.tanggalLahir;
        if (txtTanggalTerbit != null)   txtTanggalTerbit.text   = kupon.tanggalTerbit;

        if (imgFotoKupon != null)
        {
            imgFotoKupon.sprite = kupon.fotoKupon;
            imgFotoKupon.enabled = kupon.fotoKupon != null;
        }

        if (imgStempel != null) imgStempel.sprite = kupon.stempelSprite;
        panelKupon.SetActive(true);
    }

    public void Putuskan(bool diterima)
    {
        panelKupon.SetActive(false);
        npcAktif?.OnKeputusanKupon(diterima);
    }
}