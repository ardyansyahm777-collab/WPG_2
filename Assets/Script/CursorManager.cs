using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Cursor Textures")]
    public Texture2D normalCursor;
    public Texture2D hoverCursor;
    public Texture2D clickCursor;

    [Header("Settings")]
    public Vector2 hotSpot = Vector2.zero;

    private bool _isHovering = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        SetNormal();
    }

    void Update()
    {
        // Logika klik global
        if (Input.GetMouseButtonDown(0))
        {
            SetClick();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Jika saat melepas klik kita masih hovering di atas objek, balik ke Hover.
            // Jika tidak, balik ke Normal.
            if (_isHovering) SetHover();
            else SetNormal();
        }
    }

    // Fungsi internal untuk mengganti status hover dari skrip lain
    public void SetHoverState(bool hovering)
    {
        _isHovering = hovering;
        
        // Jangan ganti kursor ke Hover jika user sedang menahan klik (sedang drag)
        if (!Input.GetMouseButton(0))
        {
            if (hovering) SetHover();
            else SetNormal();
        }
    }

    public void SetNormal() => Cursor.SetCursor(normalCursor, hotSpot, CursorMode.Auto);
    public void SetHover() => Cursor.SetCursor(hoverCursor, hotSpot, CursorMode.Auto);
    public void SetClick() => Cursor.SetCursor(clickCursor, hotSpot, CursorMode.Auto);
}