using UnityEngine;

public class ShowObject : MonoBehaviour
{
    public string title;
    [TextArea(3, 10)]
    public string description;
    public Sprite objectSprite;
    public Sprite[] daftarTanggal;
    
    // Referensi ke UIManager
    public UIManager uiManager;

    // Fungsi ini yang akan kita hubungkan ke Button
    public void TriggerShow()
    {
        if (uiManager != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.paper);
            uiManager.ShowInfo(title, description, objectSprite);

            // Tambahan: jika objek ini adalah kalender
            if (gameObject.name == "calendar")
            {
                // Kalender sekarang hanya update tampilan hari
                Object.FindFirstObjectByType<PergantianKalender>()?.UpdateKalender();

                // Jika ingin memicu transisi hari (misalnya setelah cutscene)
                // panggil TransisiHariBerikutnya() dari GameManager atau PergantianKalender
                // Object.FindFirstObjectByType<GameManager>()?.NPCFinishedTurn();
            }
        }
        else
        {
            Debug.LogError("UIManager belum di-assign di ShowObject pada " + gameObject.name);
        }
    }
}
