using UnityEngine;

public class DragAndDropRaycast : MonoBehaviour
{
    private bool isDragging = false;
    private Transform objectToDrag;
    private Vector3 offset;

    void Update()
    {
        // Konversi posisi mouse ke world position (2D)
        Vector3 mousePos = Input.mousePosition;
        float distance = -Camera.main.transform.position.z; // jarak dari kamera ke bidang z=0
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distance));
        worldMousePos.z = 0; // paksa z = 0 untuk objek 2D

        // Saat tombol ditekan: deteksi objek
        if (Input.GetMouseButtonDown(0))
        {
            // Gunakan OverlapPoint untuk mendeteksi collider di posisi mouse
            Collider2D hit = Physics2D.OverlapPoint(worldMousePos);
            if (hit != null)
            {
                isDragging = true;
                objectToDrag = hit.transform;
                // Hitung offset: posisi objek - posisi mouse (agar objek tidak loncat)
                offset = objectToDrag.position - worldMousePos;
            }
        }

        // Saat tombol dilepas: lepaskan objek
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            objectToDrag = null;
        }

        // Selama drag, update posisi objek
        if (isDragging && objectToDrag != null)
        {
            objectToDrag.position = worldMousePos + offset;
        }
    }
}