using UnityEngine;

public class ItemAsli : MonoBehaviour
{
    public GameManager gameManager; // Drag GameManager dari scene ke sini

    private void OnMouseDown()
    {
        if (!gameObject.name.Contains("(Clone)"))
    {
        // Buat clone dari objek ini
        GameObject clone = Instantiate(gameObject, transform.position, Quaternion.identity);

        // Tambahkan script DragClone ke clone
        DragClone drag = clone.AddComponent<DragClone>();
        drag.gameManager = gameManager;

        // Beri nama unik agar tidak masuk ke loop if ini lagi saat diklik
        clone.name = gameObject.name + "(Clone)";
    }
    else 
    {
        Debug.Log("Ini sudah objek clone, tidak akan menduplikasi lagi.");
    }
    }
}