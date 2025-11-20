using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    public Camera cam;
    public Material highlightMaterial;
    public bool isDragging = false;

    private ObjectController selectedObj;
    private Plane groundPlane;
    

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        groundPlane = new Plane(Vector3.up, Vector3.zero); // y = 0 plane
    }

    void Update()
    {
        HandleSelection();
        HandleDragging();
    }

    ObjectController CheckHit()
    {
        ObjectController oc = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            oc = hit.collider.GetComponent<ObjectController>();
            
        }
        Debug.Log(oc == null ? "Hit Object is null!" : "Hit Object found.");

        return oc;
    }

    void SetDragState(bool newValue)
    {
        isDragging = newValue;       
    }

    void HandleSelection()
    {
        // On mouse down, try selecting
        if (Input.GetMouseButtonDown(0))
        {
            ObjectController oc = CheckHit();
            if ( oc != null)
            {
                // Deselect previous
                if (selectedObj != null)
                    selectedObj.Deselect();
                Debug.Log("Check Point");
                selectedObj = oc;
                // selectedObj.SetHighlightMaterial(highlightMaterial);
                selectedObj.Select();

                SetDragState(true);
            }
        }
            
        // Release drag
        if (Input.GetMouseButtonUp(0))
        {
            SetDragState(false);
        }
    }

    void HandleDragging()
    {
        if (!isDragging || selectedObj == null)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            if (selectedObj.tag == "HoleB") 
                selectedObj.transform.position = new Vector3(hitPoint.x, 0.5f, hitPoint.z);
            else 
                selectedObj.transform.position = new Vector3(hitPoint.x, 0f, hitPoint.z);
            
            CSGManager cm = FindFirstObjectByType<CSGManager>();
            cm.Rebuild();
        }
    }
}
