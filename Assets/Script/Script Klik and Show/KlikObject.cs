using UnityEngine;
using UnityEngine.UI;

public class KlikObject : MonoBehaviour
{
    public UIManager uiManager;
    public Text infoText;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                ShowObject obj = hit.collider.GetComponent<ShowObject>();

                if(obj != null)
                {
                    infoText.text = obj.description;
                }
            }
        }
    }
}
