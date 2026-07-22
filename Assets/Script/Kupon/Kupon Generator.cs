using UnityEngine;

public class KuponGenerator : MonoBehaviour
{
    [Header("Stempel")]
    public Sprite stempelAsli;
    public Sprite stempelPalsuMirip; // dibuat mirip tapi beda warna/posisi

    [Header("Peluang Kupon Palsu (0-1)")]
    [Range(0f, 1f)] public float peluangPalsuDasar = 0.25f;
    [Range(0f, 0.2f)] public float tambahanPerHari = 0.05f;

    static readonly string[] contohNama = {
        "Siti Aminah", "Budi Santoso", "Rahmawati", "Joko Prasetyo", "Nur Halimah", "Andi Wijaya"
    };

    public KuponInfo Generate(int hariSekarang)
    {
        KuponInfo k = new KuponInfo();
        k.namaPengungsi = contohNama[Random.Range(0, contohNama.Length)];

        float peluangPalsu = Mathf.Clamp01(peluangPalsuDasar + (hariSekarang - 1) * tambahanPerHari);
        k.asli = Random.value >= peluangPalsu;

        int nomorAcak = Random.Range(1000, 9999);
        k.nomorRegistrasi = $"PSK-{nomorAcak}-2004";
        k.tanggalTerbit   = $"{hariSekarang + 26}-12-2004"; // sesuaikan dgn tanggal kalender di gameplay
        k.stempelSprite   = stempelAsli;
        k.kecacatan       = KuponInfo.JenisKecacatan.TidakAda;

        if (!k.asli)
        {
            int jenis = Random.Range(0, 3);
            switch (jenis)
            {
                case 0:
                    k.kecacatan = KuponInfo.JenisKecacatan.NomorFormatSalah;
                    k.nomorRegistrasi = $"PSK-{nomorAcak}-2003"; // tahun keliru
                    break;
                case 1:
                    k.kecacatan = KuponInfo.JenisKecacatan.TanggalKadaluarsa;
                    k.tanggalTerbit = $"{hariSekarang + 20}-12-2004"; // tanggal jauh beda
                    break;
                case 2:
                    k.kecacatan = KuponInfo.JenisKecacatan.StempelPalsu;
                    k.stempelSprite = stempelPalsuMirip;
                    break;
            }
        }
        return k;
    }
}