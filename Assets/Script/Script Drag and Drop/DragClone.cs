using UnityEngine;
using UnityEngine.EventSystems;

public class DragClone : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameManager gameManager;
    public GameObject dropZone;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // cek apakah mouse berada di dalam DropZone
        if (!RectTransformUtility.RectangleContainsScreenPoint(
            dropZone.GetComponent<RectTransform>(),
            Input.mousePosition,
            eventData.pressEventCamera))
        {
            Destroy(gameObject); // hancurkan clone
        }
    }
}
