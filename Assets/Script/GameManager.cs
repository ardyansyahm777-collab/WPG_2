using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
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
}
