using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Shader Property Names")]
    [SerializeField] private string thicknessProp = "_Thickness";
    [SerializeField] private string outlineColorProp = "_OutlineColor";

    [Header("Color Settings")]
    [SerializeField] private Color outlineColor = Color.white;

    [Header("Hover Settings")]
    [SerializeField] private float activeThickness = 2f;
    [SerializeField] private float fadeDuration = 0.2f;

    private float currentThickness = 0f;
    private Material targetMaterial;
    private Coroutine fadeCoroutine;
    private bool isUI = false;

    void Awake()
    {
        Image img = GetComponent<Image>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (img != null) 
        { 
            // Membuat instance material baru agar tidak mengubah semua objek yang pakai material sama
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
            // Inisialisasi awal: outline mati
            targetMaterial.SetFloat(thicknessProp, 0f);
            targetMaterial.SetColor(outlineColorProp, outlineColor);
        }
    }

    // Trigger untuk UI (Canvas)
    public void OnPointerEnter(PointerEventData eventData) => StartFade(activeThickness);
    public void OnPointerExit(PointerEventData eventData) => StartFade(0f);

    // Trigger untuk Sprite (World Space)
    void OnMouseEnter() { if (!isUI) StartFade(activeThickness); }
    void OnMouseExit() { if (!isUI) StartFade(0f); }

    private void StartFade(float targetT)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetT));
    }

    private IEnumerator FadeRoutine(float targetT)
    {
        float elapsedTime = 0f;
        float startT = currentThickness;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            // Transisi halus nilai ketebalan outline saja
            currentThickness = Mathf.Lerp(startT, targetT, t);

            if (targetMaterial != null)
            {
                targetMaterial.SetFloat(thicknessProp, currentThickness);
            }
            yield return null;
        }

        currentThickness = targetT;
        targetMaterial.SetFloat(thicknessProp, currentThickness);
    }
}