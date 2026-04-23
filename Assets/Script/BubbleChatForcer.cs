using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BubbleChatForcer : MonoBehaviour
{
    private Canvas bubbleCanvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Tambah Canvas override di BubbleChat object ini
        bubbleCanvas = GetComponent<Canvas>();
        if (bubbleCanvas == null)
            bubbleCanvas = gameObject.AddComponent<Canvas>();

        bubbleCanvas.overrideSorting = true;
        bubbleCanvas.sortingOrder = 100;

        // Tambah GraphicRaycaster kalau belum ada
        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        // Tambah CanvasGroup untuk kontrol alpha
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Sembunyikan via alpha, bukan SetActive
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    // Panggil fungsi ini dari NPC.cs untuk tampilkan bubble
    public void Tampilkan()
    {
        gameObject.SetActive(true);
        StartCoroutine(ForceShow());
    }

    // Panggil fungsi ini untuk sembunyikan bubble
    public void Sembunyikan()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        gameObject.SetActive(false);
    }

    IEnumerator ForceShow()
    {
        // Sembunyikan dulu via alpha (object tetap aktif)
        canvasGroup.alpha = 0f;

        // Paksa rebuild layout dulu sebelum ditampilkan
        Canvas.ForceUpdateCanvases();
        var allRects = GetComponentsInChildren<RectTransform>(true);
        foreach (var r in allRects)
            LayoutRebuilder.ForceRebuildLayoutImmediate(r);

        // Tunggu 2 frame
        yield return null;
        yield return null;

        // Rebuild lagi setelah 2 frame
        Canvas.ForceUpdateCanvases();
        foreach (var r in allRects)
            LayoutRebuilder.ForceRebuildLayoutImmediate(r);

        // Baru tampilkan via alpha
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        RectTransform rt = GetComponent<RectTransform>();
        Debug.Log($"[BubbleChat] anchoredPos: {rt.anchoredPosition}");
        Debug.Log($"[BubbleChat] sizeDelta: {rt.sizeDelta}");
        Debug.Log($"[BubbleChat] worldPos: {rt.position}");
        Debug.Log($"[BubbleChat] parent: {transform.parent?.name}");
        Debug.Log($"[BubbleChat] canvas sortingOrder: {bubbleCanvas.sortingOrder}");
        Debug.Log($"[BubbleChat] alpha: {canvasGroup.alpha}");
    }
}