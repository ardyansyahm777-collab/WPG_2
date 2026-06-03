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

    public void OnBeginDrag(PointerEventData eventData) 
    { 
        canvasGroup.blocksRaycasts = false; 
        canvasGroup.alpha = 0.6f; 
    }

    public void OnDrag(PointerEventData eventData) 
    { 
        rectTransform.anchoredPosition += eventData.delta; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Cek apakah item dilepas di dalam area Meja (Drop Zone)
        if (dropZone != null && RectTransformUtility.RectangleContainsScreenPoint(
            dropZone.GetComponent<RectTransform>(), Input.mousePosition, eventData.pressEventCamera))
        {
            // --- HANYA BERI TAG TANPA MENAMBAH STOK DI PLAYER SERVE ---
            if (gameObject.tag != "ItemDimeja") 
            {
                gameObject.tag = "ItemDimeja"; 
                Debug.Log($"Item {tipeItem} ditaruh di meja. (Stok harian tidak bertambah karena sudah diberikan di awal hari)");
            }
        }
        else 
        { 
            // Jika ditarik keluar dari meja atau dilepas di luar drop zone, item dihancurkan
            // Tidak perlu mengurangi stok GameDataManager karena saat ditaruh pun tidak menambah stok
            Destroy(gameObject); 
        }
    }
}