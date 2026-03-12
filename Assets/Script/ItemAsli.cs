using UnityEngine;

public class ItemAsli : MonoBehaviour
{
    public GameManager gameManager; // Drag GameManager dari scene ke sini

    private void OnMouseDown()
    {
        // Buat clone dari objek ini
        GameObject clone = Instantiate(gameObject, transform.position, Quaternion.identity);

        // Tambahkan script DragClone ke clone
        DragClone drag = clone.AddComponent<DragClone>();
        drag.gameManager = gameManager;

        // Opsional: beri nama berbeda agar tidak bingung
        clone.name = gameObject.name + "(Clone)";
    }
}