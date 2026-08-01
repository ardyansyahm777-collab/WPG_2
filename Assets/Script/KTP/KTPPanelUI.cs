using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel KTP sebagai dokumen referensi untuk verifikasi silang data.
/// </summary>
public class KTPPanelUI : MonoBehaviour
{
    [Header("UI References (Sesuai Hierarchy KTP)")]
    public GameObject panelKTP;
    public TextMeshProUGUI txtNama;
    public TextMeshProUGUI txtKotaTglLahir; // Slot untuk GameObject 'kota_tgl lahir'
    public TextMeshProUGUI txtGenderUmur;   // Slot untuk GameObject 'gender, umur'
    public TextMeshProUGUI txtNik;          // Slot untuk GameObject 'nik'
    public Image imgFotoKTP;

    void Awake()
    {
        if (panelKTP != null) panelKTP.SetActive(false);
    }

    public void Tampilkan(KTPInfo ktp)
    {
        if (ktp == null)
        {
            Sembunyikan();
            return;
        }

        // 1. Set Nama
        if (txtNama != null) 
            txtNama.text = ktp.nama;

        // 2. Gabung Kota Asal dan Tanggal Lahir (Format: BANDA ACEH, 12-04-1980)
        if (txtKotaTglLahir != null) 
            txtKotaTglLahir.text = $"{ktp.asalDaerah.ToUpper()}, {ktp.tanggalLahir}";

        // 3. Gabung Gender dan Umur (Format: LAKI-LAKI, 32 / PEREMPUAN, 32)
        if (txtGenderUmur != null) 
        {
            string genderStr = (ktp.jenisKelamin == GenderType.Pria) ? "LAKI-LAKI" : "PEREMPUAN";
            txtGenderUmur.text = $"{genderStr}, {ktp.umur}";
        }

        // 4. Set NIK
        if (txtNik != null) 
            txtNik.text = ktp.nik;

        // 5. Set Foto KTP
        if (imgFotoKTP != null)
        {
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