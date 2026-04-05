using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverHightlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Shader Property Names")]
    [SerializeField] private string mainColorRef = "_MainColor";
    [SerializeField] private string thicknessRef = "_Thickness";
    [SerializeField] private string outlineColorRef = "_OutlineColor";

    [Header("Hover Settings")]
    [SerializeField] private float activeThickness = 2f;
    [SerializeField] private Color highlightColor = Color.red; // Warna highlight kayu
    [SerializeField] private Color outlineColor = Color.white;   // Warna garis tepi
    [SerializeField] private float fadeDuration = 0.2f;

    private float currentThickness = 0f;
    private Color currentHighlightColor;
    private Color originalHighlightColor = Color.white; // Default kayu (Putih = normal)
    
    private Material targetMaterial;
    private Coroutine fadeCoroutine;
    private bool isUI = false;

    void Awake()
    {
        Image img = GetComponent<Image>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (img != null) 
        { 
            // Membuat instance material unik agar tidak mempengaruhi objek lain
            img.material = new Material(img.material); 
            targetMaterial = img.material; 
            isUI = true; 
        }
        else if (sr != null) 
        { 
            // SpriteRenderer.material otomatis membuat instance unik
            targetMaterial = sr.material; 
            isUI = false; 
        }

        if (targetMaterial != null)
        {
            // Ambil warna asli SEBELUM diubah-ubah
            if (targetMaterial.HasProperty(mainColorRef))
                originalHighlightColor = targetMaterial.GetColor(mainColorRef);
            
            // Paksa reset saat start agar tidak merah
            currentHighlightColor = originalHighlightColor;
            targetMaterial.SetFloat(thicknessRef, 0f);
            targetMaterial.SetColor(mainColorRef, originalHighlightColor);
        }
    }

    // TRIGGER EVENT
    public void OnPointerEnter(PointerEventData eventData) => StartFade(activeThickness, highlightColor);
    public void OnPointerExit(PointerEventData eventData) => StartFade(0f, originalHighlightColor);

    void OnMouseEnter() { if (!isUI) StartFade(activeThickness, highlightColor); }
    void OnMouseExit() { if (!isUI) StartFade(0f, originalHighlightColor); }

    private void StartFade(float targetT, Color targetH)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetT, targetH));
    }

    private IEnumerator FadeRoutine(float targetT, Color targetH)
    {
        float elapsedTime = 0f;
        float startT = currentThickness;
        Color startH = currentHighlightColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            currentThickness = Mathf.Lerp(startT, targetT, t);
            currentHighlightColor = Color.Lerp(startH, targetH, t);

            if (targetMaterial != null)
            {
                // Update ketebalan garis tepi
                targetMaterial.SetFloat(thicknessRef, currentThickness);
                // Update highlight kayu (MainColor)
                targetMaterial.SetColor(mainColorRef, currentHighlightColor);
                // Pastikan warna outline tetap sesuai pilihan
                targetMaterial.SetColor(outlineColorRef, outlineColor);
            }
            yield return null;
        }

        currentThickness = targetT;
        currentHighlightColor = targetH;
        targetMaterial.SetFloat(thicknessRef, currentThickness);
        targetMaterial.SetColor(mainColorRef, currentHighlightColor);
    }
}