using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC : MonoBehaviour
{
    public KebutuhanSet kebutuhan;
    public Image avatarImage;
    public TextMeshProUGUI logistikText;
    public TextMeshProUGUI firstAidText;
    public float moveSpeed = 8f;

    private RectTransform rect;
    private Vector2 targetPos;
    private Animator anim;
    private bool sedangKeluar = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rect = GetComponent<RectTransform>();
        targetPos = rect.anchoredPosition;
    }

    void Update()
    {
        if (!sedangKeluar)
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, Time.deltaTime * moveSpeed);
    }

    public void SetKebutuhan(KebutuhanSet data)
    {
        kebutuhan = data;
        if (logistikText != null) logistikText.text = kebutuhan.logistik.ToString();
        if (firstAidText != null) firstAidText.text = kebutuhan.firstAid.ToString();
    }

    public void SetVisual(Sprite img) { if (avatarImage != null) avatarImage.sprite = img; }
    public void SetTargetPos(Vector2 pos) => targetPos = pos;
    public bool CekTerpenuhi(int l, int f) => l >= kebutuhan.logistik && f >= kebutuhan.firstAid;

    public void TriggerKeluar()
    {
        if (sedangKeluar) return;
        sedangKeluar = true;
        if (anim != null) anim.SetTrigger("Exit");
        Destroy(gameObject, 4f);
    }
}