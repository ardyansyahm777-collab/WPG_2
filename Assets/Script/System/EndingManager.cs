using UnityEngine;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance { get; private set; }

    public enum EndingType
    {
        None,
        EndingA_TheBureaucrat,    
        EndingB_TheSilentHero,    
        EndingC_TheMartyr,        
        EndingD_TheCorruptedShore  
    }

    [Header("Threshold Batas Poin Ending")]
    public int minHighCompliance = 10;
    public int minHighHumanity = 10;
    public int minHighCorruption = 5;

    [Header("Tipe Ending Yang Berhasil Dicapai")]
    public EndingType currentEnding = EndingType.None;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public EndingType EvaluasiEndingHari7()
    {
        GameDataManager data = GameDataManager.Instance;
        if (data == null) return EndingType.None;

        if (data.corruptionPoint >= minHighCorruption)
        {
            currentEnding = EndingType.EndingD_TheCorruptedShore;
        }
        else if (data.compliancePoint >= minHighCompliance && data.humanityPoint < minHighHumanity)
        {
            currentEnding = EndingType.EndingA_TheBureaucrat;
        }
        else if (data.humanityPoint >= minHighHumanity)
        {
            currentEnding = EndingType.EndingB_TheSilentHero;
        }
        else
        {
            currentEnding = EndingType.EndingA_TheBureaucrat;
        }

        return currentEnding;
    }
}