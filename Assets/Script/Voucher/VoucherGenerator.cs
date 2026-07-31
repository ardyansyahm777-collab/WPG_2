using System.Collections.Generic;
using UnityEngine;

public class VoucherGenerator : MonoBehaviour
{
    [Header("Database Sprite Voucher")]
    [Tooltip("Isi tiap kombinasi Jenis (Logistik/Medis) x Kondisi (Bersih/Kotor) dengan sprite-nya masing-masing.")]
    public List<VoucherSpriteEntry> daftarSpriteVoucher = new List<VoucherSpriteEntry>();

    [Header("Peluang Voucher Kotor (0-1)")]
    [Range(0f, 1f)] public float peluangKotorDasar = 0.25f;
    [Range(0f, 0.2f)] public float tambahanPerHari = 0.05f;

    /// <summary>
    /// Generate voucher untuk NPC. nomorRegistrasiKupon WAJIB diisi dengan nomor
    /// registrasi Kupon (Kartu Bantuan) NPC yang sama, supaya kedua dokumen
    /// punya nomor identik (bisa dicocokkan pemain).
    /// </summary>
    public VoucherInfo Generate(int hariSekarang, string nomorRegistrasiKupon)
    {
        VoucherInfo v = new VoucherInfo();
        v.nomorRegistrasi = nomorRegistrasiKupon;

        // Tentukan jenis voucher secara acak
        v.jenis = (Random.value < 0.5f)
            ? VoucherInfo.JenisVoucher.Logistik
            : VoucherInfo.JenisVoucher.Medis;

        // Hitung peluang kotor berdasarkan hari (makin lama makin sering kotor)
        float peluangKotor = Mathf.Clamp01(peluangKotorDasar + (hariSekarang - 1) * tambahanPerHari);
        v.kondisi = (Random.value < peluangKotor)
            ? VoucherInfo.KondisiVoucher.Kotor
            : VoucherInfo.KondisiVoucher.Bersih;

        v.voucherSprite = CariSprite(v.jenis, v.kondisi);

        return v;
    }

    private Sprite CariSprite(VoucherInfo.JenisVoucher jenis, VoucherInfo.KondisiVoucher kondisi)
    {
        if (daftarSpriteVoucher != null)
        {
            foreach (VoucherSpriteEntry entry in daftarSpriteVoucher)
            {
                if (entry.jenis == jenis && entry.kondisi == kondisi)
                {
                    return entry.sprite;
                }
            }
        }

        Debug.LogWarning($"[VoucherGenerator] Sprite untuk kombinasi {jenis}/{kondisi} tidak ditemukan di 'Daftar Sprite Voucher'!");
        return null;
    }
}