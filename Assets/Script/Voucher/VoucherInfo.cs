using UnityEngine;

[System.Serializable]
public class VoucherInfo
{
    [Tooltip("WAJIB SAMA dengan nomorRegistrasi milik KuponInfo (Kartu Bantuan) NPC yang sama.")]
    public string nomorRegistrasi;

    [Tooltip("Sprite utuh tiket voucher (visual bersih/kotor sudah termasuk di dalam sprite-nya).")]
    public Sprite voucherSprite;

    public enum JenisVoucher { Logistik, Medis }
    public enum KondisiVoucher { Bersih, Kotor }

    [Header("Klasifikasi")]
    public JenisVoucher jenis;
    public KondisiVoucher kondisi = KondisiVoucher.Bersih;
}