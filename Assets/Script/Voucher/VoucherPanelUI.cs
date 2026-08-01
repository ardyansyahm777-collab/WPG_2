using System.Collections.Generic;
using UnityEngine;

public class VoucherPanelUI : MonoBehaviour
{
    [Header("Panel Utama")]
    public GameObject panelVoucherContainer;

    [Header("Prefab Spawn Voucher")]
    [Tooltip("Drag Prefab Item Voucher (yang memiliki skrip VoucherItemUI) ke sini")]
    public GameObject voucherPrefab;

    [Tooltip("Container tempat menampung voucher yang di-spawn (misal GameObject 'voucher' di Hierarchy)")]
    public Transform voucherParentTransform;

    private List<GameObject> activeVoucherInstances = new List<GameObject>();

    void Awake()
    {
        if (panelVoucherContainer != null) panelVoucherContainer.SetActive(false);
    }

    public void TampilkanSemua(List<VoucherInfo> listVoucher)
    {
        ClearPreviousVouchers();

        if (listVoucher == null || listVoucher.Count == 0)
        {
            Sembunyikan();
            return;
        }

        // PAKSA AKTIFKAN GAMEOBJECT / CONTAINER
        if (panelVoucherContainer != null) panelVoucherContainer.SetActive(true);
        gameObject.SetActive(true); 

        Transform parentTarget = (voucherParentTransform != null) ? voucherParentTransform : transform;

        foreach (VoucherInfo vInfo in listVoucher)
        {
            if (voucherPrefab != null)
            {
                GameObject newVoucherObj = Instantiate(voucherPrefab, parentTarget);
                
                VoucherItemUI itemUI = newVoucherObj.GetComponent<VoucherItemUI>();
                if (itemUI != null)
                {
                    itemUI.Setup(vInfo);
                }

                activeVoucherInstances.Add(newVoucherObj);
            }
        }
    }

    public void Sembunyikan()
    {
        ClearPreviousVouchers();
        if (panelVoucherContainer != null) panelVoucherContainer.SetActive(false);
    }

    private void ClearPreviousVouchers()
    {
        foreach (GameObject vObj in activeVoucherInstances)
        {
            if (vObj != null) Destroy(vObj);
        }
        activeVoucherInstances.Clear();
    }
}