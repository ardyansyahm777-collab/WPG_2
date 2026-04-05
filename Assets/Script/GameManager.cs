using UnityEngine;
using UnityEngine.UI; // penting untuk Text
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Text scoreText;   // referensi ke komponen Text di UI
    private int score = 0;

    public void AddScore(int points)
    {
    Debug.Log("AddScore dipanggil! Skor sekarang: " + score);
    score += points;
    scoreText.text = "Score: " + score;
    }

    void Start()
    {
        // Jalankan penunda saat scene GamePlay dimulai
        StartCoroutine(DelayedMusic());
    }

    IEnumerator DelayedMusic()
    {
        // Tunggu 3 detik (di sini layar bisa blank atau cutscene)
        yield return new WaitForSeconds(3f);

        if (AudioManager.Instance != null)
        {
            // Mainkan kembali musiknya
            AudioManager.Instance.musicSource.clip = AudioManager.Instance.background;
            AudioManager.Instance.musicSource.Play();
        }
    }
}