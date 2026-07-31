using UnityEngine;

/// <summary>
/// Satu entry sprite voucher, dipasangkan dengan keterangan jenis (Logistik/Medis)
/// dan kondisi (Bersih/Kotor). Assign di Inspector lewat List di VoucherGenerator -
/// tiap kombinasi jenis+kondisi butuh 1 sprite sendiri (karena visualnya beda total,
/// bukan cuma tint warna).
/// </summary>
[System.Serializable]
public class VoucherSpriteEntry
{
    public VoucherInfo.JenisVoucher jenis;
    public VoucherInfo.KondisiVoucher kondisi;
    public Sprite sprite;
}