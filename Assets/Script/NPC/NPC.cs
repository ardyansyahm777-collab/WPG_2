using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public KebutuhanSet kebutuhan;

    public SpriteRenderer spriteRenderer;

    public Sprite spriteLogistik1;
    public Sprite spriteLogistik2;
    public Sprite spriteCampur;
    public Sprite spriteFirstAid;

    public void SetKebutuhan(KebutuhanSet k)
    {
        kebutuhan = k;
        UpdateSprite();
    }

    void UpdateSprite()
    {
        if(kebutuhan.logistik == 1 && kebutuhan.firstAid == 0)
        {
            spriteRenderer.sprite = spriteLogistik1;
        }
        else if(kebutuhan.logistik == 2)
        {
            spriteRenderer.sprite = spriteLogistik2;
        }
        else if(kebutuhan.logistik == 1 && kebutuhan.firstAid == 1)
        {
            spriteRenderer.sprite = spriteCampur;
        }
        else if(kebutuhan.firstAid == 1)
        {
            spriteRenderer.sprite = spriteFirstAid;
        }
    }

    public bool CekTerpenuhi(int logistikPlayer, int firstAidPlayer)
    {
        return logistikPlayer >= kebutuhan.logistik &&
               firstAidPlayer >= kebutuhan.firstAid;
    }
}