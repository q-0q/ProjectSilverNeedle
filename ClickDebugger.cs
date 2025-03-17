using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDebugger : MonoBehaviour
{
    void Update()
    {
        // Check if the user clicks on something with the left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // If we hit something, print it
                Debug.Log("Clicked on: " + hit.collider.gameObject.name);
                PrintHierarchyPath(hit.collider.gameObject);
            }
            else
            {
                // Check if any UI element was clicked using the EventSystem
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                // Raycast against the UI
                var raycastResults = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, raycastResults);

                if (raycastResults.Count > 0)
                {
                    // Print the name of the UI object clicked
                    Debug.Log("Clicked on UI element: " + raycastResults[0].gameObject.name);
                    PrintHierarchyPath(raycastResults[0].gameObject);
                }
                else
                {
                    Debug.Log("No UI element clicked.");
                }
            }
        }
    }

    // Prints the full hierarchy path to the object
    private void PrintHierarchyPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;

        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        Debug.Log("Object hierarchy path: " + path);
    }
}
