using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class KebutuhanGenerator : MonoBehaviour
{
    public List<KebutuhanSet> kemungkinan;

    void Awake()
    {
        kemungkinan = new List<KebutuhanSet>()
        {
            new KebutuhanSet { logistik = 1, firstAid = 0 },
            new KebutuhanSet { logistik = 2, firstAid = 0 },
            new KebutuhanSet { logistik = 1, firstAid = 1 },
            new KebutuhanSet { logistik = 0, firstAid = 1 }
        };
    }

    public KebutuhanSet GetRandom()
    {
        return kemungkinan[Random.Range(0, kemungkinan.Count)];
    }
}
