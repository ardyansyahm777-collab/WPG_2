using UnityEngine;

public class DragScript : MonoBehaviour
{
    Vector3 mousePosition;
    RaycastHit2D raycastHit2D;
    Transform clickObject;

    bool isMouseDown = false;

    void Start()
    {

    }
    
    
    void Update()
    {
        mousePosition = Input.mousePosition;

        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            raycastHit2D = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            clickObject = raycastHit2D ? raycastHit2D.transform : null;
        }

        if (Input.GetMouseButtonUp(0))
        {
           if (clickObject)
            {
                clickObject.GetComponent<SpriteRenderer>().color = Color.white;
                clickObject = null; // reset supaya tidak nyangkut
            }
        }
        if (isMouseDown && clickObject)
        {
            raycastHit2D = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            if (raycastHit2D)
            {
                if (clickObject.GetInstanceID() == raycastHit2D.transform.GetInstanceID())
                {
                    clickObject.GetComponent<SpriteRenderer>().color = Color.blue;

                    return;
                }
            }
            clickObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        
    }
}