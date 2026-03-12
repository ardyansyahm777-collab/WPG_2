using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    private Color originalColor;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr) originalColor = sr.color;
    }

    void OnMouseEnter()
    {
        if (sr) sr.color = Color.red;
    }

    void OnMouseExit()
    {
        if (sr) sr.color = originalColor;
    }
}