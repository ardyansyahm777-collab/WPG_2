using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct DialogLine
{
    public string namaKarakter;
    [TextArea(3, 5)] public string isiDialog;
}

public class VisualNovelManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtNama;
    public TextMeshProUGUI txtDialog;
    public GameObject panelDialog;

    [Header("Story Settings")]
    public DialogLine[] ceritaTutorial;
    public float typingSpeed = 0.04f;
    public string nextSceneName = "CutScene"; // Menuju ke scene DayCard

    private int indexDialog = 0;
    private bool sedangMengetik = false;
    private string dialogAktifLengkap = "";

    void Start()
    {
        indexDialog = 0;
        MulaiDialog();
    }

    void Update()
    {
        // Klik mouse kiri atau tombol Space untuk melanjutkan dialog
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (sedangMengetik)
            {
                // Jika sedang mengetik lalu diklik, langsung tampilkan seluruh teks line tersebut
                StopAllCoroutines();
                txtDialog.text = dialogAktifLengkap;
                sedangMengetik = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    void MulaiDialog()
    {
        if (ceritaTutorial == null || ceritaTutorial.Length == 0)
        {
            SelesaiTutorial();
            return;
        }
        TampilkanLine(indexDialog);
    }

    void TampilkanLine(int index)
    {
        txtNama.text = ceritaTutorial[index].namaKarakter;
        dialogAktifLengkap = ceritaTutorial[index].isiDialog;
        StartCoroutine(TypeDialog(dialogAktifLengkap));
    }

    IEnumerator TypeDialog(string dialog)
    {
        sedangMengetik = true;
        txtDialog.text = "";
        foreach (char huruf in dialog)
        {
            txtDialog.text += huruf;
            yield return new WaitForSeconds(typingSpeed);
        }
        sedangMengetik = false;
    }

    public void NextLine()
    {
        indexDialog++;
        if (indexDialog < ceritaTutorial.Length)
        {
            TampilkanLine(indexDialog);
        }
        else
        {
            SelesaiTutorial();
        }
    }

    /// <summary>
    /// Fungsi untuk tombol SKIP atau ketika dialog VN sudah habis dibaca.
    /// </summary>
    public void SelesaiTutorial()
    {
        Debug.Log("[VN System] Tutorial selesai atau di-skip. Melompat ke Day 1.");
        // Set transisi global ke Hari 1 secara manual agar DayCard memuat "DAY 1"
        DayTransitionManager.MulaiTransisiPertama(1);
        SceneManager.LoadScene(nextSceneName);
    }
}