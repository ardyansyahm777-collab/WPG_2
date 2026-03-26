using UnityEngine;
using UnityEngine.UI;


public class ButtonHover : MonoBehaviour
{
    public Image image;
    [SerializeField] private Color hoverColor;
    private Color originalColor;
    [SerializeField] private GameObject Arrow;

    void Start()
    {
        originalColor = image.color;
    }
    public void pointerEnter()
    {
        Arrow.SetActive(true);
        image.color = hoverColor;
    }

    public void pointerExit()
    {
        Arrow.SetActive(false);
        image.color = originalColor;
    }

    public void settingButton()
    {
        
    }
}
