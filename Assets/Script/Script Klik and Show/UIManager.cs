using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject panel;
    public Text titleText;
    public Text descriptionText;

    void Start()
    {
        panel.SetActive(false);
    }

    public void ShowInfo(string title, string desc)
    {
        panel.SetActive(true);
        titleText.text = title;
        descriptionText.text = desc;
    }

    public void HideInfo()
    {
        panel.SetActive(false);
    }
}
