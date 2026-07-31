using UnityEngine;
using UnityEngine.EventSystems;

// Script ini langsung dipasang pada Game Object yang ingin digerakkan
public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 offset;

    void Awake()
    {
        // Ambil komponen RectTransform dan Canvas jika objek ini adalah elemen UI
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
    }

    // ==========================================
    // 1. LOGIKA UNTUK SPRITE RENDERER (WORLD SPACE)
    // ==========================================
    private void OnMouseDown()
    {
        // Hanya jalan jika BUKAN elemen UI (Sprite Renderer)
        if (rectTransform == null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            offset = transform.position - mouseWorldPos;
        }
    }

    private void OnMouseDrag()
    {
        // Hanya jalan jika BUKAN elemen UI (Sprite Renderer)
        if (rectTransform == null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            transform.position = mouseWorldPos + offset;
        }
    }

    // ==========================================
    // 2. LOGIKA UNTUK UI IMAGE (CANVAS)
    // ==========================================
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rectTransform != null)
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localMousePos
            );
            offset = (Vector3)localMousePos - rectTransform.anchoredPosition3D;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform != null)
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localMousePos
            );
            rectTransform.anchoredPosition = localMousePos - (Vector2)offset;
        }
    }
}