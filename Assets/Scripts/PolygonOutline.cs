using System.Collections.Generic;
using UnityEngine;

// Generates a child GameObject rendering only the polygon boundary edges (outline edges)
// of the mesh attached to this GameObject's MeshFilter, using lines.
[RequireComponent(typeof(MeshFilter))]
public class PolygonOutline : MonoBehaviour
{
    public Material lineMaterial;

    public string outlineObjectName = "PolygonOutlineLines";

    public GameObject outlineObject;

    void Start()
    {

        

        if (lineMaterial == null)
        {
            Shader shader = Shader.Find("Unlit/Color");
            if (shader == null)
            {
                Debug.LogError("Unlit/Color shader not found!");
                return;
            }
            lineMaterial = new Material(shader);
            lineMaterial.color = new Color(1f, 0.47f, 0.21f);
        }

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogError("[PolygonOutline] No MeshFilter or Mesh found on this GameObject.");
            return;
        }

        GenerateOutline(mf.sharedMesh);
    }

    private void GenerateOutline(Mesh mesh)
    {
        // Edge dictionary: key = unordered vertex index pair, value = number of triangles sharing edge
        Dictionary<(int, int), int> edgeUsage = new Dictionary<(int, int), int>();

        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        // Count how many triangles share each edge
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            AddEdge(edgeUsage, v0, v1);
            AddEdge(edgeUsage, v1, v2);
            AddEdge(edgeUsage, v2, v0);
        }

        List<Vector3> lineVertices = new List<Vector3>();
        List<int> lineIndices = new List<int>();

        // Collect edges used by exactly one triangle (boundary edges)
        foreach (var kvp in edgeUsage)
        {
            if (kvp.Value == 1)
            {
                var (a, b) = kvp.Key;

                // Add line vertices (transform local positions)
                lineVertices.Add(vertices[a]);
                lineVertices.Add(vertices[b]);

                int idx = lineVertices.Count;
                lineIndices.Add(idx - 2);
                lineIndices.Add(idx - 1);
            }
        }

        // Create mesh for lines
        Mesh lineMesh = new Mesh
        {
            name = $"{name}_PolygonOutlineMesh"
        };
        lineMesh.SetVertices(lineVertices);
        lineMesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
        lineMesh.RecalculateBounds();

        // Create or reuse child GameObject for outline
        if (outlineObject != null)
            Destroy(outlineObject);

        outlineObject = new GameObject(outlineObjectName);
        outlineObject.transform.SetParent(transform, false);

        MeshFilter outlineMF = outlineObject.AddComponent<MeshFilter>();
        outlineMF.mesh = lineMesh;

        MeshRenderer outlineMR = outlineObject.AddComponent<MeshRenderer>();
        outlineMR.material = lineMaterial;

        outlineMR.enabled = false;
    }

    private void AddEdge(Dictionary<(int, int), int> dict, int v1, int v2)
    {
        var edge = v1 < v2 ? (v1, v2) : (v2, v1);
        if (dict.TryGetValue(edge, out int count))
            dict[edge] = count + 1;
        else
            dict[edge] = 1;
    }
}
