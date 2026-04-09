using UnityEngine;
using UnityEngine.EventSystems;

public class CursorInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Beritahu manager bahwa kita sedang hovering
        CursorManager.Instance.SetHoverState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Beritahu manager bahwa kita sudah keluar
        CursorManager.Instance.SetHoverState(false);
    }
}