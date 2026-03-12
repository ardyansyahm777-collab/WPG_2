using UnityEngine;
using UnityEngine.UI; // penting untuk Text

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
}