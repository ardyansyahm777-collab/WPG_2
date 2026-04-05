using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragClone : MonoBehaviour, IPointerDownHandler
{
    public GameManager gameManager;
    private Vector3 offset;
    private bool isDragging = true; 
    private bool initialized = false;

    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        // Hitung offset awal
        offset = transform.position - GetMousePosition();
        initialized = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartDragging();
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject()) // Pastikan tidak tertutup UI lain
        {
            StartDragging();
        }
    }

    private void StartDragging()
    {
        isDragging = true;
        offset = transform.position - GetMousePosition();
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = GetMousePosition() + offset;

            // Berhenti drag saat tombol mouse dilepas
            // initialized memastikan kita tidak tidak sengaja melepas di frame pertama saat instantiate
            if (initialized && Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                // Log dihapus sesuai permintaan
            }
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;

        if (rectTransform != null && canvas != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return mousePos;
            
            Vector3 worldPoint;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, mousePos, canvas.worldCamera, out worldPoint);
            return worldPoint;
        }

        float distance = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distance));
        worldMousePos.z = 0;
        return worldMousePos;
    }
}