using UnityEngine;

public class DragClone : MonoBehaviour
{
    public GameManager gameManager;
    private bool isDragging = true; // langsung drag setelah dibuat
    private Vector3 offset;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red; // opsional: bedakan warna clone

        // Hitung offset agar tidak melompat
        HitungOffset();
    }

    private void HitungOffset()
    {
        Vector3 mousePos = Input.mousePosition;
        float distance = -Camera.main.transform.position.z; // asumsi kamera di -10
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distance));
        worldMousePos.z = 0;
        offset = transform.position - worldMousePos;
    }

    private void Update()
    {
        if (isDragging)
        {
            // Update posisi mengikuti mouse
            Vector3 mousePos = Input.mousePosition;
            float distance = -Camera.main.transform.position.z;
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distance));
            worldMousePos.z = 0;
            transform.position = worldMousePos + offset;

            // Deteksi jika mouse dilepas
            if (Input.GetMouseButtonUp(0))
            {
                Release();
            }
        }
    }

    private void Release()
{
    isDragging = false;

    // Debug posisi clone
    Debug.Log("Posisi clone saat release: " + transform.position);

    Collider2D hit = Physics2D.OverlapPoint(transform.position);
    if (hit != null)
    {
        Debug.Log("Objek terkena: " + hit.name + ", tag: " + hit.tag);
        if (hit.CompareTag("TrashBin"))
        {
            Debug.Log("TAG TRASHBIN TERDETEKSI! Menambah skor...");
            if (gameManager != null)
                gameManager.AddScore(10);
            else
                Debug.LogError("gameManager di DragClone belum diisi!");
        }
    }
    else
    {
        Debug.Log("Tidak ada collider di posisi ini.");
    }

    Destroy(gameObject);
}
}