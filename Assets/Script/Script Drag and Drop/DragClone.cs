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

        bool diDalamMeja = dropZone != null && RectTransformUtility.RectangleContainsScreenPoint(
            dropZone.GetComponent<RectTransform>(), Input.mousePosition, eventData.pressEventCamera);

        if (diDalamMeja)
        {
            // Taruh di meja: hanya beri tag, TIDAK ubah stok sama sekali
            gameObject.tag = "ItemDimeja";
        }
        else
        {
            // Dilepas di luar meja: hancurkan saja, TIDAK ubah stok
            Destroy(gameObject);
        }
    }
}