using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemAsli : MonoBehaviour, IPointerDownHandler
{
    [Header("Visual Settings")]
    public Sprite spriteUntukClone;
    public Transform containerShow;

    [Header("Drop Settings")]
    // Tarik objek 'area_bantuan' ke slot ini di Inspector
    public GameObject dropZoneArea; 

    [Header("Item Data")]
    public KebutuhanType tipeItem;
    public int jumlahItem = 1;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Pastikan kita tidak meng-clone objek yang sudah berupa clone
        if (!gameObject.name.Contains("(Clone)")) ExecuteClone();
    }

    private void ExecuteClone()
    {
        // Safety check agar tidak error jika lupa pasang container
        if (containerShow == null) return;

        GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);
        clone.transform.SetParent(containerShow, true);
        clone.SetActive(true);
        clone.name = gameObject.name + "(Clone)";
        AudioManager.Instance.PlaySFX(AudioManager.Instance.pop);
        

        // Atur visual clone
        Image img = clone.GetComponent<Image>();
        if (img != null) 
        { 
            img.sprite = spriteUntukClone; 
            img.SetNativeSize(); 
            // PENTING: Pastikan Raycast Target aktif agar bisa di-drag
            img.raycastTarget = true; 
        }

        // Pasang dan atur script DragClone
        DragClone drag = clone.AddComponent<DragClone>();
        
        // MENGISI DROPZONE: Sekarang mengambil langsung dari variabel dropZoneArea
        drag.dropZone = this.dropZoneArea; 
        
        drag.tipeItem = this.tipeItem;
        drag.jumlahItem = this.jumlahItem;
        
        
        // Jika masih null, tampilkan peringatan di Console
        if (drag.dropZone == null) 
        {
            Debug.LogError("Drop Zone Area belum diisi di Inspector ItemAsli!");
        }
    }
}