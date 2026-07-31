using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 
using System.Collections;
using System.Collections.Generic;

// --- STRUCT DI LUAR KELAS ABOUTMENUCONTROLLER ---
[System.Serializable]
public struct CharacterExpression
{
    public string expressionName; 
    public Sprite expressionSprite; 
}

public class AboutMenuController : MonoBehaviour
{
    public static AboutMenuController Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text answerTextUI;
    [SerializeField] private GameObject answerBubble;

    [Header("Highlight Config (Simple, No Custom Shader)")]
    [Tooltip("Komponen Outline bawaan Unity. Kalau kosong, akan otomatis diambil/ditambahkan dari characterImage.")]
    [SerializeField] private Outline characterOutline;
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float highlightThickness = 4f;
    [Tooltip("Warna tint saat kartu bersentuhan dengan karakter (redup/gelap)")]
    [SerializeField] private Color hoverTintColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    private Color normalTintColor = Color.white;

    [Header("Database Ekspresi")]
    [SerializeField] private List<CharacterExpression> characterExpressions = new List<CharacterExpression>();

    [Header("Typing Effect Config")]
    [SerializeField] private float typingSpeed = 0.03f;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        // Set Singleton Instance
        Instance = this;
    }

    private void Start()
    {
        if (characterImage != null)
        {
            normalTintColor = characterImage.color;

            // Kalau belum di-assign lewat Inspector, coba ambil / tambahkan otomatis
            if (characterOutline == null)
            {
                characterOutline = characterImage.GetComponent<Outline>();
                if (characterOutline == null)
                {
                    characterOutline = characterImage.gameObject.AddComponent<Outline>();
                }
            }
        }

        TriggerHighlight(false);
    }

    /// <summary>
    /// Mengatur efek outline dan tingkat kegelapan karakter saat kartu bersentuhan (hover) vs normal.
    /// Menggunakan Image.color (tint) + komponen Outline bawaan Unity, tanpa custom shader.
    /// </summary>
    public void TriggerHighlight(bool active)
    {
        if (characterImage != null)
        {
            characterImage.color = active ? hoverTintColor : normalTintColor;
        }

        if (characterOutline != null)
        {
            characterOutline.enabled = active;
            characterOutline.effectColor = outlineColor;
            characterOutline.effectDistance = active
                ? new Vector2(highlightThickness, -highlightThickness)
                : Vector2.zero;
        }
    }


    public void OnCardDropped(DraggableQuestionCard card)
    {
        TriggerHighlight(false);

        if (card != null && card.data != null)
        {
            Debug.Log($"<b>[AboutMenuController]</b> Menerima kartu: '{card.data.questionText}'");
            ProcessQuestion(card.data);
            card.OnSuccessfullyDropped();
        }
    }

    private void ProcessQuestion(QuestionData data)
    {
        Sprite matchedSprite = GetExpressionSprite(data.expressionName);
        if (matchedSprite != null && characterImage != null)
        {
            characterImage.sprite = matchedSprite;
        }

        if (answerBubble != null) answerBubble.SetActive(true);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(data.answerText));
    }

    private Sprite GetExpressionSprite(string nameToFind)
    {
        if (characterExpressions != null && characterExpressions.Count > 0)
        {
            foreach (var item in characterExpressions)
            {
                if (!string.IsNullOrEmpty(item.expressionName) && 
                    item.expressionName.Equals(nameToFind, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (item.expressionSprite != null) return item.expressionSprite;
                }
            }

            if (characterExpressions[0].expressionSprite != null)
                return characterExpressions[0].expressionSprite;
        }

        if (characterImage != null) return characterImage.sprite;
        return null;
    }

    private IEnumerator TypeText(string textToType)
    {
        answerTextUI.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            answerTextUI.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public List<string> GetExpressionNames()
    {
        List<string> names = new List<string>();
        if (characterExpressions != null)
        {
            foreach (var item in characterExpressions)
            {
                if (!string.IsNullOrEmpty(item.expressionName))
                {
                    names.Add(item.expressionName);
                }
            }
        }
        return names;
    }

    public void keMainMenu()
    {
        // Tambahkan null check pada Button_Manager.Instance
        if (Button_Manager.Instance != null)
        {
            Button_Manager.Instance.ButtonClick();
        }
        else
        {
            Debug.LogWarning("[AboutMenuController] Button_Manager.Instance tidak ditemukan di scene ini!");
        }

        // Hentikan musik menu
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.Stop();
        }

        // Transisi ke MainMenu
        if (DayTransitionManager.Instance != null)
        {
            DayTransitionManager.Instance.transisiScene("MainMenu");
        }
        else
        {
            Debug.LogWarning("[AboutMenuController] DayTransitionManager tidak ditemukan, load scene langsung.");
            // Fallback langsung ke MainMenu
            SceneManager.LoadScene("MainMenu");
        }
    }
}