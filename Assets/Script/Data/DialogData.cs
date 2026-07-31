using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogData", menuName = "NPC/Dialog Data")]
public class DialogData : ScriptableObject
{
    [Header("1. DIALOG CERITA UNIK / SPONTAN (UTAMA)")]
    [TextArea(3, 10)] 
    public string dialogUtamaSpontan;

    [Header("2. DIALOG GENERIK (FALLBACK JIKA DIALOG UTAMA KOSONG)")]
    [TextArea(3, 10)] public string[] dialogLogistik;
    [TextArea(3, 10)] public string[] dialogFirstAid;
    [TextArea(3, 10)] public string[] dialogKeduanya;
}