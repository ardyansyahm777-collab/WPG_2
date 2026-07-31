using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel ini TIDAK punya tombol Terima/Tolak - KTP cuma dokumen referensi yang
/// tampil BERSAMAAN dengan panel Kupon & Voucher supaya pemain bisa membandingkan
/// datanya sendiri untuk mendeteksi kejanggalan. Kalau NPC tidak membawa KTP
/// sama sekali (ktp == null), panel ini otomatis disembunyikan.
/// </summary>
public class KTPPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelKTP;
    public TextMeshProUGUI txtNama;
    public TextMeshProUGUI txtUmur;
    public TextMeshProUGUI txtTanggalLahir;
    public TextMeshProUGUI txtNik;
    public TextMeshProUGUI txtJenisKelamin;
    public TextMeshProUGUI txtAsalDaerah;
    public TextMeshProUGUI txtTanggalKedaluwarsa;
    public Image imgFotoKTP;

    void Awake()
    {
        if (panelKTP != null) panelKTP.SetActive(false);
    }

    public void Tampilkan(KTPInfo ktp)
    {
        // NPC yang tidak membawa KTP (mis. Teuku Rahman, dokumennya hanyut) ->
        // pastikan panel disembunyikan, bukan cuma dibiarkan di state terakhir.
        if (ktp == null)
        {
            Sembunyikan();
            return;
        }

        if (txtNama != null) txtNama.text = ktp.nama;
        if (txtUmur != null) txtUmur.text = $"{ktp.umur} Tahun";
        if (txtTanggalLahir != null) txtTanggalLahir.text = ktp.tanggalLahir;
        if (txtNik != null) txtNik.text = ktp.nik;
        if (txtJenisKelamin != null) txtJenisKelamin.text = ktp.jenisKelamin == GenderType.Pria ? "Laki-laki" : "Perempuan";
        if (txtAsalDaerah != null) txtAsalDaerah.text = ktp.asalDaerah;
        if (txtTanggalKedaluwarsa != null) txtTanggalKedaluwarsa.text = ktp.tanggalKedaluwarsa;

        if (imgFotoKTP != null)
        {
            // Foto robek -> kosongkan gambar (biar kelihatan "tidak bisa dicocokkan")
            imgFotoKTP.sprite = ktp.fotoRobek ? null : ktp.fotoKTP;
            imgFotoKTP.enabled = !ktp.fotoRobek && ktp.fotoKTP != null;
        }

        if (panelKTP != null) panelKTP.SetActive(true);
    }

    public void Sembunyikan()
    {
        if (panelKTP != null) panelKTP.SetActive(false);
    }
}