using UnityEngine;

public class HoverScript : MonoBehaviour
{
    private Transform prevHovered;
    private SpriteRenderer prevSR;

    void Update()
    {
        // Konversi posisi mouse ke world point
        Vector3 mousePos = Input.mousePosition;
        float distance = -Camera.main.transform.position.z; // jarak kamera ke bidang z=0
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distance));
        worldPos.z = 0;

        // Deteksi collider di titik tersebut
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        Transform currentHovered = hit ? hit.transform : null;

        // Kembalikan warna objek sebelumnya jika berbeda
        if (prevHovered != null && prevHovered != currentHovered)
        {
            SpriteRenderer sr = prevHovered.GetComponent<SpriteRenderer>();
            if (sr) sr.color = Color.white;
        }

        // Warnai objek yang sedang di-hover
        if (currentHovered != null)
        {
            SpriteRenderer sr = currentHovered.GetComponent<SpriteRenderer>();
            if (sr) sr.color = Color.red;
        }

        prevHovered = currentHovered;
    }
}