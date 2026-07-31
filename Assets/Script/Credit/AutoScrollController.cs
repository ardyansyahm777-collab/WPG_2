using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoScrollController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("Scroll Config")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 0.05f; // Kecepatan scroll otomatis ke atas
    [SerializeField] private bool autoScrollActive = true;

    private bool isDragging = false;

    private void Awake()
    {
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();
    }

    private void Update()
    {
        // Jika auto scroll aktif dan pemain sedang tidak men-drag list
        if (autoScrollActive && !isDragging && scrollRect != null)
        {
            // ScrollRect.verticalNormalizedPosition bernilai 1 (paling atas) dan 0 (paling bawah).
            // Untuk bergerak dari BAWAH ke ATAS, nilainya kita kurangi secara bertahap.
            scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime;

            // Jika sudah mencapai paling bawah (0), loop kembali ke paling atas (1)
            if (scrollRect.verticalNormalizedPosition <= 0f)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }
    }

    // Pause auto-scroll sementara saat kursor/sentuhan sedang men-drag kartu
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
}