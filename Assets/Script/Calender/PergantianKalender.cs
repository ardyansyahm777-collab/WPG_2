using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; 

public class PergantianKalender : MonoBehaviour
{
    public Image calendarImage;
    public Sprite[] daftarTanggal;
    public TextMeshProUGUI tanggalText;
    public GameObject laporanUI;
    public int HariSekarang => hariSekarang;



    private int hariSekarang = 0;

    void Start()
    {
        StartCoroutine(InitDelay());
    }

    IEnumerator InitDelay()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateKalender();
    }

    public void UpdateKalender()
    {
        if (hariSekarang < daftarTanggal.Length && calendarImage != null)
            calendarImage.sprite = daftarTanggal[hariSekarang];

        if (tanggalText != null)
            tanggalText.text = $"Hari ke-{hariSekarang + 1}";
    }

    public void TransisiHariBerikutnya()
    {
        
        hariSekarang++;

        if (hariSekarang < daftarTanggal.Length)
        {
            calendarImage.sprite = daftarTanggal[hariSekarang];
            Debug.Log("Sprite diganti ke: " + daftarTanggal[hariSekarang].name);
        }
        else
        {
            Debug.LogWarning("Index hariSekarang melebihi jumlah sprite!");
        }

        UpdateKalender();
    }
}
