using UnityEngine;

public enum GenderType { Pria, Wanita }
public enum UsiaType { Anak, Dewasa, Lansia }

[System.Serializable]
public class NPCRandomProfile
{
    public Sprite avatarSprite;
    public GenderType gender;
    public UsiaType usia;
}

public class DocumentDataGenerator : MonoBehaviour
{
    public static DocumentDataGenerator Instance { get; private set; }

    [Header("Pool Visual NPC (Assign Sprite di Inspector)")]
    public NPCRandomProfile[] daftarProfileNPC;

    // --- DATABASE NAMA BERDASARKAN GENDER & USIA ---
    private string[] namaPriaAnak = { "Muhammad Rizky", "Aulia Zikri", "Farhan Ramadhan" };
    private string[] namaWanitaAnak = { "Siti Rahmah", "Cut Annisa", "Nabila Putri" };

    private string[] namaPriaDewasa = { "Teuku Iskandar", "Zulkarnaen", "Munawar", "Rahmat Hidayat", "Teuku Umar" };
    private string[] namaWanitaDewasa = { "Cut Maulida", "Suriyani", "Husna Juwita", "Cut Dhien" };

    private string[] namaPriaLansia = { "Teungku Sulaiman", "Nyak Umar", "Pak Usman" };
    private string[] namaWanitaLansia = { "Hajjah Asmawati", "Nyak Cut Aminah" };

    // --- DATABASE TANGGAL LAHIR BERDASARKAN USIA ---
    private string[] tglAnak = { "14-05-1995", "02-08-1997", "19-11-1993", "08-03-1996" };   // 7-11 Tahun
    private string[] tglDewasa = { "12-04-1978", "05-09-1982", "18-01-1971", "30-07-1980" }; // 24-33 Tahun
    private string[] tglLansia = { "01-01-1938", "17-08-1942", "10-05-1939", "28-11-1944" }; // 60-66 Tahun

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Mengambil 1 Profile Visual secara acak dari Pool
    /// </summary>
    public NPCRandomProfile GetRandomProfile()
    {
        if (daftarProfileNPC != null && daftarProfileNPC.Length > 0)
        {
            return daftarProfileNPC[Random.Range(0, daftarProfileNPC.Length)];
        }
        return null;
    }

    /// <summary>
    /// Menghasilkan Data Dokumen yang PRESISI sesuai Gender & Usia dari Profile Visual yang terpilih.
    /// Dipertahankan untuk kompatibilitas - secara internal sekarang memanggil GenerateNPCDocuments().
    /// </summary>
    public KuponInfo GenerateMatchingDocument(NPCRandomProfile profile, int hariSekarang)
    {
        GenerateNPCDocuments(profile, hariSekarang, out KuponInfo kupon, out KTPInfo ktp);
        return kupon;
    }

    /// <summary>
    /// Menghasilkan KuponInfo DAN KTPInfo sekaligus dari nama & tanggal lahir yang SAMA,
    /// supaya kedua dokumen konsisten satu sama lain (dipakai untuk verifikasi silang).
    /// Kalau di-generate terpisah, nama/tanggal lahir bisa ke-roll acak berbeda walau
    /// profile-nya sama.
    /// </summary>
    public void GenerateNPCDocuments(NPCRandomProfile profile, int hariSekarang, out KuponInfo kupon, out KTPInfo ktp)
    {
        string nama;
        string tanggalLahir;
        AmbilNamaDanTanggalLahir(profile, out nama, out tanggalLahir);

        kupon = new KuponInfo();
        kupon.namaPengungsi = nama;
        kupon.tanggalTerbit = tanggalLahir;

        int nomorAcak = Random.Range(1000, 9999);
        kupon.nomorRegistrasi = $"PSK-{nomorAcak}-2004";
        kupon.asli = true;

        ktp = new KTPInfo();
        ktp.nama = nama;
        ktp.tanggalLahir = tanggalLahir;
        ktp.umur = HitungUmur(tanggalLahir);

        // Data tambahan - default-nya SELALU VALID (16 digit, daerah lokal, belum
        // kedaluwarsa, foto sesuai profile). Kecacatan (NIK kurang digit, KTP
        // kedaluwarsa, mismatch gender/daerah/foto) khusus di-author manual per
        // Story NPC lewat Inspector, bukan lewat generator acak ini.
        ktp.nik = $"11{Random.Range(10, 99)}{Random.Range(100000, 999999)}{Random.Range(1000, 9999)}"; // 16 digit
        ktp.jenisKelamin = profile != null ? profile.gender : GenderType.Pria;
        ktp.asalDaerah = "Banda Aceh";
        ktp.tanggalKedaluwarsa = $"{hariSekarang + 26}-12-2009"; // 5 tahun ke depan, masih valid
        ktp.fotoKTP = profile != null ? profile.avatarSprite : null;
        ktp.fotoRobek = false;
    }

    private void AmbilNamaDanTanggalLahir(NPCRandomProfile profile, out string nama, out string tanggalLahir)
    {
        if (profile != null)
        {
            // 1. Tentukan Nama Sesuai Gender & Usia
            if (profile.gender == GenderType.Pria)
            {
                if (profile.usia == UsiaType.Anak) nama = GetRandomName(namaPriaAnak);
                else if (profile.usia == UsiaType.Lansia) nama = GetRandomName(namaPriaLansia);
                else nama = GetRandomName(namaPriaDewasa);
            }
            else // Wanita
            {
                if (profile.usia == UsiaType.Anak) nama = GetRandomName(namaWanitaAnak);
                else if (profile.usia == UsiaType.Lansia) nama = GetRandomName(namaWanitaLansia);
                else nama = GetRandomName(namaWanitaDewasa);
            }

            // 2. Tentukan Tanggal Lahir Sesuai Usia
            if (profile.usia == UsiaType.Anak) tanggalLahir = GetRandomName(tglAnak);
            else if (profile.usia == UsiaType.Lansia) tanggalLahir = GetRandomName(tglLansia);
            else tanggalLahir = GetRandomName(tglDewasa);
        }
        else
        {
            nama = "Warga Tanpa Nama";
            tanggalLahir = "01-01-1980";
        }
    }

    [Header("Konteks Waktu Narasi (untuk hitung umur KTP)")]
    [Tooltip("Tahun 'sekarang' di dalam cerita, dipakai untuk menghitung umur dari tanggal lahir.")]
    public int tahunNarasiSekarang = 2004;

    private int HitungUmur(string tanggalLahir)
    {
        // Format tanggalLahir: "dd-MM-yyyy"
        string[] bagian = tanggalLahir.Split('-');
        if (bagian.Length == 3 && int.TryParse(bagian[2], out int tahunLahir))
        {
            return Mathf.Max(0, tahunNarasiSekarang - tahunLahir);
        }
        return 0;
    }

    private string GetRandomName(string[] listNama)
    {
        if (listNama == null || listNama.Length == 0) return "Warga Aceh";
        return listNama[Random.Range(0, listNama.Length)];
    }

    
}