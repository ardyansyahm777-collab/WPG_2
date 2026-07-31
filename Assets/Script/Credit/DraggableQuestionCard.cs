using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableQuestionCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{
    [Header("Data & UI")]
    public QuestionData data;
    [SerializeField] private TMP_Text cardText;

    [Header("Visual Container (Pivot Tengah)")]
    [Tooltip("Masukkan Transform dari Child Object 'CardVisual' yang ber-Pivot X: 0.5 Y: 0.5")]
    [SerializeField] private Transform cardVisualTransform;

    [Header("Movement Config")]
    public float moveSpeed = 60f;
    public float destroyPosY = 600f;

    [Header("Hover Shake Effect")]
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeAngle = 8f;
    [SerializeField] private int shakeVibrato = 6;

    private Coroutine hoverShakeCoroutine;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isDragging = false;
    private bool droppedSuccessfully = false;
    private Vector3 dragOffset;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        // Fallback jika lupa di-assign di Inspector
        if (cardVisualTransform == null)
        {
            cardVisualTransform = transform;
        }
    }

    public void SetupCard(QuestionData newData, float speed, float maxY)
    {
        data = newData;
        moveSpeed = speed;
        destroyPosY = maxY;

        if (cardText != null) cardText.text = data.questionText;
    }

    private void Update()
    {
        if (!isDragging)
        {
            rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

            if (rectTransform.anchoredPosition.y >= destroyPosY)
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;

        PlayHoverShakeOnce();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (hoverShakeCoroutine != null)
        {
            StopCoroutine(hoverShakeCoroutine);
            cardVisualTransform.localRotation = Quaternion.identity;
            hoverShakeCoroutine = null;
        }

        isDragging = true;
        droppedSuccessfully = false;
        transform.SetAsLastSibling();

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.7f;
        }

        dragOffset = transform.position - (Vector3)eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = (Vector3)eventData.position + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        if (droppedSuccessfully)
        {
            return;
        }

        Destroy(gameObject);
    }

    public void PlayHoverShakeOnce()
    {
        if (hoverShakeCoroutine != null) StopCoroutine(hoverShakeCoroutine);
        hoverShakeCoroutine = StartCoroutine(HoverShakeRoutine());
    }

    /// <summary>
    /// Shake diputar pada cardVisualTransform (Pivot 0.5, 0.5), 
    /// sehingga goyangannya seimbang antara sisi kiri dan kanan!
    /// </summary>
    private System.Collections.IEnumerator HoverShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / shakeDuration;
            float damper = 1f - progress;
            
            float angle = Mathf.Sin(progress * shakeVibrato * Mathf.PI * 2f) * shakeAngle * damper;

            cardVisualTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }

        cardVisualTransform.localRotation = Quaternion.identity;
        hoverShakeCoroutine = null;
    }

    public void MarkDroppedSuccessfully()
    {
        droppedSuccessfully = true;
    }

    public void OnSuccessfullyDropped()
    {
        Destroy(gameObject);
    }
}