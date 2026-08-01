using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoucherItemUI : MonoBehaviour
{
    [Header("UI References dalam Prefab Voucher")]
    public Image imgVoucher;
    public TextMeshProUGUI txtNomorRegistrasi;

    public void Setup(VoucherInfo vInfo)
    {
        if (vInfo == null) return;

        if (imgVoucher != null) 
            imgVoucher.sprite = vInfo.voucherSprite;

        if (txtNomorRegistrasi != null) 
            txtNomorRegistrasi.text = vInfo.nomorRegistrasi;
    }
}