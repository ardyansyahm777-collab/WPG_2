using System.Collections.Generic;
using UnityEngine;

public class VoucherGenerator : MonoBehaviour
{
    [Header("Database Sprite Voucher")]
    public List<VoucherSpriteEntry> daftarSpriteVoucher = new List<VoucherSpriteEntry>();

    [Header("Peluang Voucher Kotor (0-1)")]
    [Range(0f, 1f)] public float peluangKotorDasar = 0.25f;
    [Range(0f, 0.2f)] public float tambahanPerHari = 0.05f;

    /// <summary>
    /// Menghasilkan List Voucher yang disesuaikan dengan KUANTITAS KebutuhanSet milik NPC.
    /// </summary>
    public List<VoucherInfo> GenerateVouchersForNPC(int hariSekarang, string nomorRegistrasiKupon, KebutuhanSet kebutuhan)
    {
        List<VoucherInfo> listVoucher = new List<VoucherInfo>();
        if (kebutuhan == null) return listVoucher;

        float peluangKotor = Mathf.Clamp01(peluangKotorDasar + (hariSekarang - 1) * tambahanPerHari);

        // 1. Loop sebanyak jumlah logistik yang dibutuhkan (misal logistik = 2, maka buat 2 voucher)
        for (int i = 0; i < kebutuhan.logistik; i++)
        {
            VoucherInfo vLog = CreateSingleVoucher(nomorRegistrasiKupon, VoucherInfo.JenisVoucher.Logistik, peluangKotor);
            listVoucher.Add(vLog);
        }

        // 2. Loop sebanyak jumlah firstAid/medis yang dibutuhkan
        for (int i = 0; i < kebutuhan.firstAid; i++)
        {
            VoucherInfo vMed = CreateSingleVoucher(nomorRegistrasiKupon, VoucherInfo.JenisVoucher.Medis, peluangKotor);
            listVoucher.Add(vMed);
        }

        return listVoucher;
    }

    private VoucherInfo CreateSingleVoucher(string nomorRegistrasi, VoucherInfo.JenisVoucher jenis, float peluangKotor)
    {
        VoucherInfo v = new VoucherInfo();
        v.nomorRegistrasi = nomorRegistrasi;
        v.jenis = jenis;
        v.kondisi = (Random.value < peluangKotor) ? VoucherInfo.KondisiVoucher.Kotor : VoucherInfo.KondisiVoucher.Bersih;
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

        Debug.LogWarning($"[VoucherGenerator] Sprite untuk kombinasi {jenis}/{kondisi} tidak ditemukan!");
        return null;
    }
}