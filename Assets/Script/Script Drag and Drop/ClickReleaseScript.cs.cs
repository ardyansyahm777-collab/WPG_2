using UnityEngine;

public class ClickReleaseScript : MonoBehaviour
{
    Vector3 mousePosition;
    RaycastHit2D raycastHit2D;
    Transform ClickedObject;

    void Update()
    {
        mousePosition = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

        // Saat mouse ditekan
        if (Input.GetMouseButtonDown(0))
        {
            raycastHit2D = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, Mathf.Infinity);
            ClickedObject = raycastHit2D ? raycastHit2D.collider.transform : null;

            if (ClickedObject)
            {
                ClickedObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }

        // Saat mouse dilepas
        if (Input.GetMouseButtonUp(0))
        {
            if (ClickedObject)
            {
                ClickedObject.GetComponent<SpriteRenderer>().color = Color.white;
                ClickedObject = null; // reset supaya tidak nyangkut
            }
        }
    }
}
