using UnityEngine;
using UnityEngine.EventSystems;

public class DragClone : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject dropZone;
    public KebutuhanType tipeItem;
    public int jumlahItem;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData) { canvasGroup.blocksRaycasts = false; canvasGroup.alpha = 0.6f; }
    public void OnDrag(PointerEventData eventData) { rectTransform.anchoredPosition += eventData.delta; }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (dropZone != null && RectTransformUtility.RectangleContainsScreenPoint(
            dropZone.GetComponent<RectTransform>(), Input.mousePosition, eventData.pressEventCamera))
        {
            // --- CEK APAKAH INI BARU PERTAMA KALI DITARUH? ---
            if (gameObject.tag != "ItemDimeja") 
            {
                PlayerServe player = Object.FindFirstObjectByType<PlayerServe>();
                if (player != null)
                {
                    if (tipeItem == KebutuhanType.Logistik) player.logistik += jumlahItem;
                    else if (tipeItem == KebutuhanType.FirstAid) player.firstAid += jumlahItem;
                    
                    // Beri tag SETELAH stok ditambah
                    gameObject.tag = "ItemDimeja"; 
                    Debug.Log("Item baru ditaruh. Stok bertambah.");
                }
            }
            else 
            {
                Debug.Log("Item sudah ada di meja, tidak menambah stok lagi.");
            }
        }
        else 
        { 
            // Jika ditarik keluar dari meja (mungkin pemain berubah pikiran)
            // Kita harus kurangi stoknya kembali jika ia sudah pernah terhitung
            if (gameObject.tag == "ItemDimeja")
            {
                PlayerServe player = Object.FindFirstObjectByType<PlayerServe>();
                if (player != null)
                {
                    if (tipeItem == KebutuhanType.Logistik) player.logistik -= jumlahItem;
                    else if (tipeItem == KebutuhanType.FirstAid) player.firstAid -= jumlahItem;
                }
            }
            
            Destroy(gameObject); 
        }
    }
}