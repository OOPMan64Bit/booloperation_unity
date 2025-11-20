using UnityEngine;
using Parabox.CSG;

public class CSGManager : MonoBehaviour
{
    public GameObject boxA;
    public GameObject boxB;
    public GameObject holeA;
    public GameObject holeB;

    public Transform resultRoot;   // empty parent container
    public Material resultMaterial;

    private GameObject lastResult;

    void Start()
    {
        Rebuild();
    }
    
    // When any object is dragging, Called from SelectionManager.
    // Rebuilds the CSG result based on current transforms.
    public void Rebuild()
    {
        if (!boxA || !boxB || !holeA || !holeB || !resultRoot)
        {
            Debug.LogError("CSGManager: Missing references.");
            return;
        }

        // 0. Cleanup previous final result
        if (lastResult != null)
            Destroy(lastResult);

        //----------------------------------------------------------
        // 1) UNION boxA + boxB
        //----------------------------------------------------------
        // GenerateBarycentric(boxB);
        // GenerateBarycentric(boxA);

        Model unionModel = CSG.Union(boxB, boxA);

        GameObject tempGO = new GameObject();
        tempGO.transform.SetParent(resultRoot, false);
        tempGO.AddComponent<MeshFilter>().sharedMesh = unionModel.mesh;
        tempGO.AddComponent<MeshRenderer>().sharedMaterial = resultMaterial;
        tempGO.transform.localScale = new Vector3(1.0f, 0.95f, 1.0f); // to view difference

        GenerateBarycentric(tempGO);

        //----------------------------------------------------------
        // 2) SUBTRACT holeA
        //----------------------------------------------------------
        Model subAModel = CSG.Subtract(tempGO, holeA);

        tempGO.GetComponent<MeshFilter>().sharedMesh = subAModel.mesh;
        GenerateBarycentric(tempGO);

        //----------------------------------------------------------
        // 3) SUBTRACT holeB
        //----------------------------------------------------------
        Model subBModel = CSG.Subtract(tempGO, holeB);

        tempGO.GetComponent<MeshFilter>().sharedMesh = subBModel.mesh;
        GenerateBarycentric(tempGO);

        //----------------------------------------------------------
        // 4) Cleanup intermediates
        //----------------------------------------------------------

        lastResult = tempGO;
    }

    void GenerateBarycentric(GameObject go)
    {
        Mesh m = go.GetComponent<MeshFilter>().sharedMesh;

        if (m == null) return;

        int[] tris = m.triangles;
        int triangleCount = tris.Length;

        Vector3[] mesh_vertices = m.vertices;
        Vector3[] mesh_normals = m.normals;
        Vector2[] mesh_uv = m.uv;

        Vector3[] vertices = new Vector3[triangleCount];
        Vector3[] normals = new Vector3[triangleCount];
        Vector2[] uv = new Vector2[triangleCount];
        Color[] colors = new Color[triangleCount];

        for (int i = 0; i < triangleCount; i++)
        {
            vertices[i] = mesh_vertices[tris[i]];
            normals[i] = mesh_normals[tris[i]];
            uv[i] = mesh_uv[tris[i]];

            colors[i] = i % 3 == 0 ? new Color(1, 0, 0, 0) : (i % 3) == 1 ? new Color(0, 1, 0, 0) : new Color(0, 0, 1, 0);

            tris[i] = i;
        }

        Mesh barycentricMesh = new Mesh();

        barycentricMesh.Clear();
        barycentricMesh.vertices = vertices;
        barycentricMesh.triangles = tris;
        barycentricMesh.normals = normals;
        barycentricMesh.colors = colors;
        barycentricMesh.uv = uv;

        go.GetComponent<MeshFilter>().sharedMesh = barycentricMesh;
    }

}
