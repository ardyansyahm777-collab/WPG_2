using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach script ini LANGSUNG ke GameObject "karakter" (yang punya komponen Image).
/// Pastikan Image di GameObject ini punya "Raycast Target" = TRUE, kalau tidak,
/// event drop tidak akan pernah sampai ke sini.
///
/// Ini menggantikan sistem deteksi manual (Physics2D/RectTransformUtility) di
/// DraggableQuestionCard dengan sistem drag-drop resmi Unity UI, yang otomatis
/// dihitung oleh EventSystem + GraphicRaycaster milik Canvas. Jauh lebih simpel
/// dan tidak tergantung RectTransform GameObject lain (yang kemarin salah ambil).
/// </summary>
[RequireComponent(typeof(Image))]
public class CharacterDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Kosongkan untuk otomatis pakai AboutMenuController.Instance")]
    [SerializeField] private AboutMenuController controller;

    private void Awake()
    {
        Image img = GetComponent<Image>();
        if (!img.raycastTarget)
        {
            Debug.LogWarning("<color=red>[CharacterDropZone] Raycast Target di Image 'karakter' mati! Drop tidak akan terdeteksi. Aktifkan di Inspector.</color>");
        }
    }

    private AboutMenuController Controller => controller != null ? controller : AboutMenuController.Instance;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsDraggingCard(eventData)) return;

        Controller?.TriggerHighlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsDraggingCard(eventData)) return;

        Controller?.TriggerHighlight(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableQuestionCard card = GetDraggedCard(eventData);
        if (card == null) return;

        Debug.Log("<color=green>[CharacterDropZone] DROP BERHASIL DI ATAS KARAKTER!</color>");

        Controller?.TriggerHighlight(false);
        Controller?.OnCardDropped(card);

        // Beri tahu kartu bahwa drop-nya berhasil, supaya OnEndDrag di kartu
        // tidak ikut menghancurkan object-nya lagi.
        card.MarkDroppedSuccessfully();
    }

    private bool IsDraggingCard(PointerEventData eventData)
    {
        return GetDraggedCard(eventData) != null;
    }

    private DraggableQuestionCard GetDraggedCard(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null) return null;
        return eventData.pointerDrag.GetComponent<DraggableQuestionCard>();
    }
}