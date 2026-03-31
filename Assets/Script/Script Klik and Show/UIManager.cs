using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel; 
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image displayImage;

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void ShowInfo(string title, string desc, Sprite newSprite)
    {
        if (panel == null) return;

        panel.SetActive(true);
        titleText.text = title; // Menerima nama GameObject
        descriptionText.text = desc;

        if (newSprite != null)
        {
            displayImage.sprite = newSprite;
            displayImage.SetNativeSize();
        }
    }

    public void HideInfo()
    {
        panel.SetActive(false);
    }
}