using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Shader Property Names")]
    // Diubah menjadi string agar bisa mereferensi nama variabel di shader
    [SerializeField] private string mainColorProp = "_MainColor"; 
    [SerializeField] private string thicknessProp = "_Thickness";
    [SerializeField] private string outlineColorProp = "_OutlineColor";

    [Header("Color Settings")]
    [Tooltip("Warna saat tidak di-hover (atur agar tidak terlalu putih)")]
    [SerializeField] private Color idleColor = new Color(0.8f, 0.8f, 0.8f, 1f); 
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private Color outlineColor = Color.white;

    [Header("Hover Settings")]
    [SerializeField] private float activeThickness = 2f;
    [SerializeField] private float fadeDuration = 0.2f;

    private float currentThickness = 0f;
    private Color currentColor;
    
    private Material targetMaterial;
    private Coroutine fadeCoroutine;
    private bool isUI = false;

    void Awake()
    {
        Image img = GetComponent<Image>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (img != null) 
        { 
            img.material = new Material(img.material); 
            targetMaterial = img.material; 
            isUI = true; 
        }
        else if (sr != null) 
        { 
            targetMaterial = sr.material; 
            isUI = false; 
        }

        if (targetMaterial != null)
        {
            // Set ke warna idle awal agar tidak langsung putih/merah
            currentColor = idleColor;
            targetMaterial.SetFloat(thicknessProp, 0f);
            targetMaterial.SetColor(mainColorProp, idleColor);
            targetMaterial.SetColor(outlineColorProp, outlineColor);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => StartFade(activeThickness, highlightColor);
    public void OnPointerExit(PointerEventData eventData) => StartFade(0f, idleColor);

    void OnMouseEnter() { if (!isUI) StartFade(activeThickness, highlightColor); }
    void OnMouseExit() { if (!isUI) StartFade(0f, idleColor); }

    private void StartFade(float targetT, Color targetC)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetT, targetC));
    }

    private IEnumerator FadeRoutine(float targetT, Color targetC)
    {
        float elapsedTime = 0f;
        float startT = currentThickness;
        Color startC = currentColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            currentThickness = Mathf.Lerp(startT, targetT, t);
            currentColor = Color.Lerp(startC, targetC, t);

            if (targetMaterial != null)
            {
                targetMaterial.SetFloat(thicknessProp, currentThickness);
                targetMaterial.SetColor(mainColorProp, currentColor);
            }
            yield return null;
        }

        currentThickness = targetT;
        currentColor = targetC;
        targetMaterial.SetFloat(thicknessProp, currentThickness);
        targetMaterial.SetColor(mainColorProp, currentColor);
    }
}