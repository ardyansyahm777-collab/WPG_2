using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogData", menuName = "NPC/Dialog Data")]

// untuk data dialog dan sudah ditaruh didalam folder data
public class DialogData : ScriptableObject
{
    [Header("Gunakan {0} untuk angka")]
    [TextArea(3, 10)] public string[] dialogLogistik;
    [TextArea(3, 10)] public string[] dialogFirstAid;
    
    [Header("Gunakan {0} untuk Logistik, {1} untuk First Aid")]
    [TextArea(3, 10)] public string[] dialogKeduanya;
}