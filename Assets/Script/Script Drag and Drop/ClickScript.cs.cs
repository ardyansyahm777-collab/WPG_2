using UnityEngine;

public class ClickScript : MonoBehaviour
{
    Vector3 mousePosition;
    RaycastHit2D raycastHit2D;
    Transform ClickedObject;

    void Start()
    {
        
    }

    void Update()
    {
        mousePosition = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            raycastHit2D = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            ClickedObject = raycastHit2D ? raycastHit2D.collider.transform : null;

            if (ClickedObject)
            {
                if (ClickedObject.GetComponent<SpriteRenderer>() != null)
                {
                    ClickedObject.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
    }
}