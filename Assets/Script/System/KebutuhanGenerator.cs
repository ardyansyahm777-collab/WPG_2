using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DayConfig
{
    public string namaHari;
    public int jumlahTotalNPC; 
}

public class KebutuhanGenerator : MonoBehaviour
{
    public Sprite[] kumpulanGambarNPC; 
    public List<KebutuhanSet> kemungkinanKebutuhan;
    public List<DayConfig> daftarHari;
    public int indexHariSekarang = 0;

    void Awake()
    {
        // Daftar kebutuhan random
        kemungkinanKebutuhan = new List<KebutuhanSet>()
        {
            new KebutuhanSet { logistik = 1, firstAid = 0 },
            new KebutuhanSet { logistik = 2, firstAid = 0 },
            new KebutuhanSet { logistik = 1, firstAid = 1 },
            new KebutuhanSet { logistik = 0, firstAid = 1 }
        };
    }

    public int GetTotalNPC() => daftarHari[indexHariSekarang].jumlahTotalNPC;

    public KebutuhanSet GetRandomKebutuhan() => kemungkinanKebutuhan[Random.Range(0, kemungkinanKebutuhan.Count)];

    public Sprite GetRandomSprite()
    {
        if (kumpulanGambarNPC.Length == 0) return null;
        return kumpulanGambarNPC[Random.Range(0, kumpulanGambarNPC.Length)];
    }  

    public int GetTargetNPC()
    {
        return daftarHari[indexHariSekarang].jumlahTotalNPC;
    }
}