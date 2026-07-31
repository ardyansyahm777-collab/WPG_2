using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Voucher sekarang MURNI dokumen bukti pendukung (kayak KTP) - TIDAK ADA
/// tombol Terima/Tolak. Keputusan terima/tolak permintaan bantuan dilakukan
/// lewat panel Kupon (Kartu Bantuan); voucher ini cuma bukti yang harus
/// dicocokkan pemain sendiri sesuai aturan rulebook (mis. "jangan terima
/// kalau voucher tidak ada atau kotor").
/// </summary>
public class VoucherPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelVoucher;
    public TextMeshProUGUI txtNomorRegistrasi;
    public Image imgVoucher;

    void Awake()
    {
        if (panelVoucher != null) panelVoucher.SetActive(false);
    }

    public void Tampilkan(VoucherInfo voucher)
    {
        // NPC yang tidak membawa voucher -> pastikan panel tersembunyi
        if (voucher == null)
        {
            Sembunyikan();
            return;
        }

        if (txtNomorRegistrasi != null) txtNomorRegistrasi.text = voucher.nomorRegistrasi;
        if (imgVoucher != null) imgVoucher.sprite = voucher.voucherSprite;

        if (panelVoucher != null) panelVoucher.SetActive(true);
    }

    public void Sembunyikan()
    {
        if (panelVoucher != null) panelVoucher.SetActive(false);
    }
}