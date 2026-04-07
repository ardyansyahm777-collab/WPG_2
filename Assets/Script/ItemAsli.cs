using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemAsli : MonoBehaviour, IPointerDownHandler
{
    [Header("Settings")]
    public GameManager gameManager;
    public Sprite spriteUntukClone;
    public Transform containerShow;

    private void OnMouseDown()
    {
        if (!gameObject.name.Contains("(Clone)"))
        {
            ExecuteClone();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!gameObject.name.Contains("(Clone)"))
        {
            ExecuteClone();
        }
    }

    private void ExecuteClone()
    {
        if (containerShow == null) 
        {
            Debug.LogError("Container Show belum diisi di Inspector!");
            return;
        }

        GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);
        
        // Set Parent
        clone.transform.SetParent(containerShow, true);
        
        // PAKSA AKTIF: Agar clone muncul meskipun 'object_show' sedang mati
        clone.SetActive(true); 

        clone.transform.localScale = transform.localScale;
        clone.name = gameObject.name + "(Clone)";

        ApplySpriteAndSize(clone);

        DragClone drag = clone.AddComponent<DragClone>();
        drag.gameManager = gameManager;

        Destroy(clone.GetComponent<ItemAsli>());
    }

    private void ApplySpriteAndSize(GameObject clone)
    {
        if (spriteUntukClone == null) return;

        Image uiFront = clone.GetComponent<Image>();
        if (uiFront != null)
        {
            uiFront.sprite = spriteUntukClone;
            uiFront.SetNativeSize();
            RectTransform backRect = clone.transform.Find("back")?.GetComponent<RectTransform>();
            if (backRect != null)
            {
                backRect.sizeDelta = uiFront.rectTransform.sizeDelta;
            }
            return;
        }

        SpriteRenderer sr = clone.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = spriteUntukClone;
            Transform backObj = clone.transform.Find("back");
            if (backObj != null)
            {
                backObj.localScale = Vector3.one; 
            }
        }
    }
}