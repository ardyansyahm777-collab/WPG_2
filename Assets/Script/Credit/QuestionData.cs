using UnityEngine;

public enum QuestionType { Core, Technical, OutOfTopic }

[CreateAssetMenu(fileName = "NewQuestionData", menuName = "About System/Question Data")]
public class QuestionData : ScriptableObject
{
    [TextArea(2, 5)]
    public string questionText;

    [TextArea(3, 8)]
    public string answerText;

    public QuestionType type;

    [Header("Karakter Expression")]
    [ExpressionName]
    public string expressionName; 
}