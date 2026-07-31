using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SimpleQuestionSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform containerTransform;
    [SerializeField] private GameObject questionCardPrefab;

    [Header("Database Pertanyaan")]
    [SerializeField] private List<QuestionData> questionPool = new List<QuestionData>();

    [Header("Movement Config")]
    [SerializeField] private float moveSpeed = 60f;     
    [SerializeField] private float spawnPosY = -600f;   
    [SerializeField] private float destroyPosY = 600f;  
    [SerializeField] private float cardSpacing = 85f;   
    [SerializeField] private int initialAmount = 8;     

    [Header("Alignment Settings")]
    [Tooltip("Posisi X (sisi paling kiri) yang kamu inginkan untuk semua kartu")]
    [SerializeField] private float targetLeftX = -200f;

    private RectTransform lastSpawnedCard = null;

    private void Start()
    {
        for (int i = 0; i < initialAmount; i++)
        {
            SpawnCardInChain();
        }
    }

    private void Update()
    {
        if (lastSpawnedCard == null || lastSpawnedCard.anchoredPosition.y >= (spawnPosY + cardSpacing))
        {
            SpawnCardInChain();
        }
    }

    public void SpawnCardInChain()
    {
        if (questionPool == null || questionPool.Count == 0) return;

        QuestionData randomData = questionPool[Random.Range(0, questionPool.Count)];

        GameObject newCardObj = Instantiate(questionCardPrefab, containerTransform);
        newCardObj.transform.localScale = Vector3.one; // Memastikan scale tidak ter-reset saat instantiate

        RectTransform cardRect = newCardObj.GetComponent<RectTransform>();

        // 1. Hitung Posisi Y seperti logika asli kamu
        float targetY = spawnPosY;
        if (lastSpawnedCard != null)
        {
            targetY = lastSpawnedCard.anchoredPosition.y - cardSpacing;
        }

        // Set posisi Y awal
        cardRect.anchoredPosition = new Vector2(0f, targetY);
        lastSpawnedCard = cardRect;

        // 2. Setup script kartu asli milikmu
        DraggableQuestionCard cardScript = newCardObj.GetComponent<DraggableQuestionCard>();
        if (cardScript != null)
        {
            cardScript.SetupCard(randomData, moveSpeed, destroyPosY);
        }

        // 3. Hitung & Sesuaikan Posisi X agar Sisi Kiri Rata Sejajar di targetLeftX
        AlignLeftWithCenterPivot(cardRect, newCardObj);
    }

    /// <summary>
    /// Menghitung ulang lebar prefab dan menyesuaikan Pos X agar sisi paling kiri
    /// kartu tepat berada di targetLeftX walaupun Pivot X = 0.5 (Center).
    /// </summary>
    private void AlignLeftWithCenterPivot(RectTransform cardRect, GameObject cardObj)
    {
        if (cardRect == null) return;

        // Paksa TMP_Text di dalam kartu untuk update ukurannya detik ini juga
        TMP_Text tmpText = cardObj.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.ForceMeshUpdate();
        }

        // Rebuild layout agar Content Size Fitter menghitung ulang ukurannya
        LayoutRebuilder.ForceRebuildLayoutImmediate(cardRect);

        // Ambil lebar kartu
        float boxWidth = cardRect.rect.width;
        if (boxWidth <= 0f && tmpText != null)
        {
            boxWidth = tmpText.GetPreferredValues().x;
        }

        // Rumus Pos X Tengah: Target Left X + (Lebar / 2)
        float adjustedX = targetLeftX + (boxWidth * 0.5f);

        // Pertahankan Pos Y yang dihitung oleh rantai spawner, hanya update Pos X
        cardRect.anchoredPosition = new Vector2(adjustedX, cardRect.anchoredPosition.y);
    }
}