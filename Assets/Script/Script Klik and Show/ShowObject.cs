using UnityEngine;

public class ShowObject : MonoBehaviour
{
    public string title;
    [TextArea(3, 10)]
    public string description;
    public Sprite objectSprite;
    
    // Referensi ke UIManager
    public UIManager uiManager;

    // Fungsi ini yang akan kita hubungkan ke Button
    public void TriggerShow()
    {
        if (uiManager != null)
        {
            // Mengambil nama GameObject ("calendar") sebagai judul
            uiManager.ShowInfo(title, description, objectSprite);
        }
        else
        {
            Debug.LogError("UIManager belum ditarik ke script ShowObject di " + gameObject.name);
        }
    }
}