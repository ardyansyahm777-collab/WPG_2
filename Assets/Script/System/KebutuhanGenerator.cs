using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DayConfig
{
    public string namaHari;
    public int jumlahTotalNPC; 

    [Header("Bantuan Masuk Otomatis Per Hari")]
    public int pasokanLogistikHariIni;
    public int pasokanMedicHariIni;
}

public class KebutuhanGenerator : MonoBehaviour
{
    [Tooltip("Fallback jika DocumentDataGenerator tidak ditemukan di Scene")]
    public Sprite[] kumpulanGambarNPC; 
    public List<KebutuhanSet> kemungkinanKebutuhan;
    public List<DayConfig> daftarHari;
    public int indexHariSekarang = 0;

    void Awake()
    {
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

    /// <summary>
    /// Mengambil gambar acak (Digunakan sebagai Fallback untuk Story NPC atau saat Generator rusak)
    /// </summary>
    public Sprite GetRandomSprite()
    {
        // 1. Cek dari DocumentDataGenerator yang baru (menggunakan NPCRandomProfile)
        if (DocumentDataGenerator.Instance != null)
        {
            NPCRandomProfile profileAcak = DocumentDataGenerator.Instance.GetRandomProfile();
            if (profileAcak != null && profileAcak.avatarSprite != null)
            {
                return profileAcak.avatarSprite;
            }
        }

        // 2. Fallback: Gunakan kumpulanGambarNPC bawaan
        if (kumpulanGambarNPC != null && kumpulanGambarNPC.Length > 0)
        {
            return kumpulanGambarNPC[Random.Range(0, kumpulanGambarNPC.Length)];
        }

        return null;
    }  

    public int GetTargetNPC()
    {
        return daftarHari[indexHariSekarang].jumlahTotalNPC;
    }
}