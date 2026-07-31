using UnityEngine;

/// <summary>
/// KTP berfungsi sebagai dokumen REFERENSI (ground truth) untuk verifikasi
/// silang terhadap Kupon/Voucher. Tidak ada tombol Terima/Tolak untuk KTP itu
/// sendiri - pemain membandingkan data di sini dengan dokumen lain secara manual.
/// </summary>
[System.Serializable]
public class KTPInfo
{
    [Header("Data Utama")]
    public string nama;
    public int umur;
    public string tanggalLahir;

    [Header("Identitas Tambahan (untuk verifikasi silang)")]
    [Tooltip("Nomor Induk Kependudukan. Standarnya 16 digit - bisa dibuat kurang/lebih sebagai kecacatan.")]
    public string nik;
    public GenderType jenisKelamin;
    [Tooltip("Kota/daerah asal penerbitan KTP, mis. 'Banda Aceh', 'Meulaboh', 'Jawa Tengah'.")]
    public string asalDaerah;
    [Tooltip("Format dd-MM-yyyy. Kosongkan kalau KTP tidak punya tanggal kedaluwarsa.")]
    public string tanggalKedaluwarsa;

    [Header("Foto KTP")]
    public Sprite fotoKTP;
    [Tooltip("Centang kalau foto rusak/robek/basah - dipakai sebagai alasan foto tidak bisa dicocokkan.")]
    public bool fotoRobek;
}