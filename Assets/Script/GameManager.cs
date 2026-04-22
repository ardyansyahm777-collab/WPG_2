using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Loop")]
    public int currentDay = 1;
    public int maxDay = 3;

    [Header("UI")]
    public GameObject winPanel; // drag UI menang ke sini
    public void NPCFinishedTurn()
    {
        Debug.Log("<color=orange>[GameManager]</color> NPC selesai, giliran berikutnya diproses.");

        // Panggil transisi hari di PergantianKalender
        Object.FindFirstObjectByType<PergantianKalender>()?.TransisiHariBerikutnya();
    }

    void Start()
    {
        StartCoroutine(DelayedMusic());
    }
    
    IEnumerator DelayedMusic()
    {
        yield return new WaitForSeconds(3f);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.musicSource.clip = AudioManager.Instance.background;
            AudioManager.Instance.musicSource.Play();
        }
    }



    // =========================
    // 🎮 GAME LOOP SYSTEM
    // =========================
    public void NextDay()
    {
        currentDay++;

        Debug.Log("Hari sekarang: " + currentDay);

        if (currentDay > maxDay)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        Debug.Log("MENANG!");

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f; // pause game
    }

    // =========================
    // 😡 SISTEM NPC MARAH
    // =========================
    [Header("Lose Condition")]
    public int maxNPCMarah = 2;
    private int jumlahNPCMarah = 0;

    public void NPCMarah()
    {
        jumlahNPCMarah++;

        Debug.Log("NPC marah: " + jumlahNPCMarah);

        if (jumlahNPCMarah >= maxNPCMarah)
        {
            LoseGame();
        }
    }

    public void ResetMarah()
    {
        jumlahNPCMarah = 0;
        Debug.Log("Reset NPC marah di hari baru");
    }

    void LoseGame()
    {
        Debug.Log("GAME OVER!");

        Time.timeScale = 0f;

        // (opsional) tampilkan UI kalah nanti di sini
    }
}
