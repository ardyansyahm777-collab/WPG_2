using System.Collections.Generic;
using UnityEngine;

// --- DEFINISI TYPE & ENUM ---
public enum GenderType { Pria, Wanita }
public enum UsiaType { Anak, Dewasa, Lansia }

[System.Serializable]
public class NPCRandomProfile
{
    [Header("Visual Karakter Fisik (NPC di Meja)")]
    public Sprite avatarSprite;

    [Header("Visual Pasfoto KTP & Kupon")]
    public Sprite fotoKTPSprite;

    [Header("Atribut")]
    public GenderType gender;
    public UsiaType usia;
}

public class DocumentDataGenerator : MonoBehaviour
{
    public static DocumentDataGenerator Instance { get; private set; }

    [Header("Pool Visual NPC")]
    public NPCRandomProfile[] daftarProfileNPC;

    [Header("Pool Foto KTP & Kupon Khusus")]
    public List<Sprite> kumpulanFotoKTP = new List<Sprite>();

    [Header("Stempel Kupon")]
    public Sprite stempelAsli;
    public Sprite stempelPalsu;

    [Header("Peluang Kecacatan Dokumen (Dua Sisi/Mismatch)")]
    [Range(0f, 0.5f)] public float peluangTypoNama = 0.2f;      // Nama KTP != Nama Kupon
    [Range(0f, 0.5f)] public float peluangMismatchNIK = 0.2f;    // NIK KTP != NIK Kupon
    [Range(0f, 0.5f)] public float peluangFotoSalah = 0.15f;    // Foto KTP != Avatar NPC
    [Range(0f, 0.5f)] public float peluangStempelPalsu = 0.2f;   // Stempel Kupon Palsu

    // Database Kode Kecamatan di Banda Aceh (11.71.01 - 11.71.09)
    private string[] kodeKecamatanBandaAceh = {
        "117101", // Baiturrahman
        "117102", // Kuta Alam
        "117103", // Meuraxa
        "117104", // Syiah Kuala
        "117105", // Lueng Bata
        "117106", // Kuta Raja
        "117107", // Banda Raya
        "117108", // Jaya Baru
        "117109"  // Ulee Kareng
    };

    // --- DATABASE NAMA BERDASARKAN GENDER & USIA ---
    private string[] namaPriaAnak = { "Muhammad Rizky", "Aulia Zikri", "Farhan Ramadhan" };
    private string[] namaWanitaAnak = { "Siti Rahmah", "Cut Annisa", "Nabila Putri" };

    private string[] namaPriaDewasa = { "Teuku Iskandar", "Zulkarnaen", "Munawar", "Rahmat Hidayat", "Teuku Umar" };
    private string[] namaWanitaDewasa = { "Cut Maulida", "Suriyani", "Husna Juwita", "Cut Dhien" };

    private string[] namaPriaLansia = { "Teungku Sulaiman", "Nyak Umar", "Pak Usman" };
    private string[] namaWanitaLansia = { "Hajjah Asmawati", "Nyak Cut Aminah" };

    // --- DATABASE NAMA SALAH / TYPO UNTUK KECACATAN ---
    private string[] namaPalsuTypo = { "Teuku Iskandarr", "Zulkarnain", "Munawarr", "Cut Mavlida", "Suriayani" };

    // --- DATABASE TANGGAL LAHIR ---
    private string[] tglAnak = { "14-05-1995", "02-08-1997", "19-11-1993" };
    private string[] tglDewasa = { "12-04-1980", "05-09-1982", "18-01-1971" };
    private string[] tglLansia = { "01-01-1938", "17-08-1942", "10-05-1939" };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public NPCRandomProfile GetRandomProfile()
    {
        if (daftarProfileNPC != null && daftarProfileNPC.Length > 0)
        {
            return daftarProfileNPC[Random.Range(0, daftarProfileNPC.Length)];
        }
        return null;
    }

    public Sprite GetRandomFotoKTP()
    {
        if (kumpulanFotoKTP != null && kumpulanFotoKTP.Count > 0)
        {
            return kumpulanFotoKTP[Random.Range(0, kumpulanFotoKTP.Count)];
        }
        return null;
    }

    /// <summary>
    /// Menghasilkan NIK Resmi Indonesia (16 Digit) berdasarkan Kode Wilayah Banda Aceh, Tanggal Lahir, dan Gender.
    /// </summary>
    public string GenerateNIKSesuaiFormat(string tanggalLahirFormatDDMMYYYY, GenderType gender)
    {
        // 1. Ambil Kode Kecamatan Banda Aceh (6 Digit Pertama)
        string kodeWilayah = kodeKecamatanBandaAceh[Random.Range(0, kodeKecamatanBandaAceh.Length)];

        // 2. Parsel Tanggal Lahir (Digit 7-12)
        string[] bagianTgl = tanggalLahirFormatDDMMYYYY.Split('-');
        int tgl = 12, bln = 4, thn = 1980;

        if (bagianTgl.Length == 3)
        {
            int.TryParse(bagianTgl[0], out tgl);
            int.TryParse(bagianTgl[1], out bln);
            int.TryParse(bagianTgl[2], out thn);
        }

        // Aturan NIK Indonesia: Jika Wanita, Tanggal Lahir + 40
        if (gender == GenderType.Wanita)
        {
            tgl += 40;
        }

        string strTgl = tgl.ToString("D2");
        string strBln = bln.ToString("D2");
        string strThn = (thn % 100).ToString("D2"); // Ambil 2 digit terakhir tahun lahir (misal 1980 -> 80)

        // 3. Nomor Urut Pendaftaran (4 Digit Terakhir)
        string nomorUrut = Random.Range(1, 99).ToString("D4");

        return $"{kodeWilayah}{strTgl}{strBln}{strThn}{nomorUrut}";
    }

    /// <summary>
    /// Menghasilkan KuponInfo dan KTPInfo SINKRON dari DocumentDataGenerator.
    /// </summary>
    public void GenerateNPCDocuments(NPCRandomProfile profile, int hariSekarang, out KuponInfo kupon, out KTPInfo ktp)
    {
        string namaAsli;
        string tanggalLahir;
        AmbilNamaDanTanggalLahir(profile, out namaAsli, out tanggalLahir);

        GenderType genderAsli = profile != null ? profile.gender : GenderType.Pria;

        // Generate NIK Resmi 16-Digit yang valid sesuai tanggal lahir & gender
        string nikAsli = GenerateNIKSesuaiFormat(tanggalLahir, genderAsli);

        bool adaTypoNama = (Random.value < peluangTypoNama);
        bool adaMismatchNIK = (Random.value < peluangMismatchNIK);
        bool fotoKTPSalah = (Random.value < peluangFotoSalah);
        bool isStempelPalsu = (Random.value < peluangStempelPalsu);

        // Ambil Pasfoto Karakter
        Sprite fotoKarakter = (profile != null && profile.fotoKTPSprite != null) ? profile.fotoKTPSprite : GetRandomFotoKTP();
        Sprite fotoKTPFinal = fotoKTPSalah ? GetRandomFotoKTP() : fotoKarakter;

        // 1. KUPON GENERATION
        kupon = new KuponInfo();
        kupon.namaPengungsi = namaAsli;
        kupon.nik = nikAsli;                             // NIK dimasukkan ke Kupon
        kupon.tanggalLahir = tanggalLahir;              // Tanggal Lahir dimasukkan ke Kupon
        kupon.tanggalTerbit = $"{hariSekarang + 20}-12-2004";
        kupon.fotoKupon = fotoKarakter; 
        kupon.stempelSprite = isStempelPalsu ? stempelPalsu : stempelAsli;

        int nomorAcak = Random.Range(1000, 9999);
        kupon.nomorRegistrasi = $"PSK-{nomorAcak}-2004";
        
        // Kupon sah jika tidak ada typo nama, NIK cocok, foto cocok, dan stempel asli
        kupon.asli = !adaTypoNama && !adaMismatchNIK && !fotoKTPSalah && !isStempelPalsu;

        // 2. KTP GENERATION
        ktp = new KTPInfo();
        ktp.nama = adaTypoNama ? GetRandomName(namaPalsuTypo) : namaAsli;
        ktp.tanggalLahir = tanggalLahir;
        ktp.umur = HitungUmur(tanggalLahir);
        
        // NIK pada KTP (Jika Kena Mismatch, NIK KTP akan diacak berbeda dari NIK Kupon)
        ktp.nik = adaMismatchNIK ? GenerateNIKSesuaiFormat("01-01-1975", genderAsli) : nikAsli; 
        
        ktp.jenisKelamin = genderAsli;
        ktp.asalDaerah = "Banda Aceh";
        ktp.fotoKTP = fotoKTPFinal;
        ktp.fotoRobek = false;
    }

    private void AmbilNamaDanTanggalLahir(NPCRandomProfile profile, out string nama, out string tanggalLahir)
    {
        if (profile != null)
        {
            if (profile.gender == GenderType.Pria)
            {
                if (profile.usia == UsiaType.Anak) nama = GetRandomName(namaPriaAnak);
                else if (profile.usia == UsiaType.Lansia) nama = GetRandomName(namaPriaLansia);
                else nama = GetRandomName(namaPriaDewasa);
            }
            else
            {
                if (profile.usia == UsiaType.Anak) nama = GetRandomName(namaWanitaAnak);
                else if (profile.usia == UsiaType.Lansia) nama = GetRandomName(namaWanitaLansia);
                else nama = GetRandomName(namaWanitaDewasa);
            }

            if (profile.usia == UsiaType.Anak) tanggalLahir = GetRandomName(tglAnak);
            else if (profile.usia == UsiaType.Lansia) tanggalLahir = GetRandomName(tglLansia);
            else tanggalLahir = GetRandomName(tglDewasa);
        }
        else
        {
            nama = "Warga Tanpa Nama";
            tanggalLahir = "12-04-1980";
        }
    }

    [Header("Konteks Waktu Narasi")]
    public int tahunNarasiSekarang = 2004;

    private int HitungUmur(string tanggalLahir)
    {
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