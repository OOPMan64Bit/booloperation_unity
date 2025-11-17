using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public Material originalMaterial;     // Assigned automatically at Start
    public Material highlightMaterial;    // Assigned from SelectionManager

    private MeshRenderer rend;
    private bool isSelected = false;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        rend.enabled = false;

    }

    public void Select()
    {
        isSelected = true;
        SetVisibleOutline(true);
        // rend.material = highlightMaterial;
    }

    public void Deselect()
    {
        isSelected = false;
        SetVisibleOutline(false);
        // rend.material = originalMaterial;
    }

    public void SetVisibleOutline(bool show)
    {
        PolygonOutline polygonOutline = GetComponent<PolygonOutline>();
        GameObject outlineObject = polygonOutline.outlineObject;
        MeshRenderer outlineMR = outlineObject.GetComponent<MeshRenderer>();
        outlineMR.enabled = show;
    }

    public void SetHighlightMaterial(Material mat)
    {
        // highlightMaterial = mat;
    }
}
