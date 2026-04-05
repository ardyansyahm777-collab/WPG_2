using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    [Header("3D Container Reference")]
    // Tarik 'img_container' ke sini
    public RectTransform imgContainer; 

    // Referensi internal ke anak-anaknya
    private Image backImage;
    private Image frontImage;

    [Header("3D Tilt Settings")]
    public float maxRotationAngle = 35f;
    public float smoothSpeed = 10f;

    void Awake()
    {
        if (imgContainer != null)
        {
            // Mencari objek bernama 'back' dan 'front' di bawah img_container
            backImage = imgContainer.Find("back")?.GetComponent<Image>();
            frontImage = imgContainer.Find("front")?.GetComponent<Image>();
        }
    }

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (panel != null && panel.activeSelf && imgContainer != null)
        {
            HandleExtreme3DFlip();
        }
    }

    public void ShowInfo(string title, string desc, Sprite newSprite)
    {
        if (panel == null) return;

        panel.SetActive(true);
        titleText.text = title;
        descriptionText.text = desc;

        if (newSprite != null && frontImage != null && backImage != null)
        {
            // 1. Set sprite ke front saja
            frontImage.sprite = newSprite;
            
            // 2. Gunakan SetNativeSize pada front agar ukurannya pas dengan gambar asli
            frontImage.SetNativeSize();

            // 3. Samakan ukuran RectTransform 'back' dengan 'front'
            RectTransform frontRect = frontImage.GetComponent<RectTransform>();
            RectTransform backRect = backImage.GetComponent<RectTransform>();
            
            backRect.sizeDelta = frontRect.sizeDelta;

            // 4. Reset rotasi container
            imgContainer.localRotation = Quaternion.identity;
        }
    }

    public void HideInfo()
    {
        if (panel != null) panel.SetActive(false);
    }

    private void HandleExtreme3DFlip()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        float deltaX = (mousePos.x - screenCenter.x) / (Screen.width / 2f);
        float deltaY = (mousePos.y - screenCenter.y) / (Screen.height / 2f);

        float targetRotX = deltaY * maxRotationAngle;
        float targetRotY = -deltaX * maxRotationAngle;
        float targetRotZ = deltaX * (maxRotationAngle * 0.5f);

        Quaternion targetRotation = Quaternion.Euler(targetRotX, targetRotY, targetRotZ);
        imgContainer.localRotation = Quaternion.Slerp(imgContainer.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}