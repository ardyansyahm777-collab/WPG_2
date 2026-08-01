using UnityEngine;

[System.Serializable]
public class KuponInfo
{
    [Header("Data Identitas Kupon")]
    public string nomorRegistrasi;
    public string namaPengungsi;
    public string nik;           // Slot NIK untuk verifikasi
    public string tanggalLahir;  // Slot Tanggal Lahir untuk verifikasi
    public string tanggalTerbit;

    [Header("Visual")]
    public Sprite fotoKupon;
    public Sprite stempelSprite;

    [Header("Status Keabsahan")]
    public bool asli; // TRUE = sah, FALSE = palsu/mismatch

    public enum JenisKecacatan { TidakAda, NomorFormatSalah, TanggalKadaluarsa, StempelPalsu }
    public JenisKecacatan kecacatan = JenisKecacatan.TidakAda;
}