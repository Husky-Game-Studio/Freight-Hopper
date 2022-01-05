using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), ExecuteInEditMode]
public class ChunkMesh : MonoBehaviour
{
    [SerializeField] private Vector2 size;
    [SerializeField] private Vector2Int resolution; // min 2 for both
    private MeshFilter meshFilter;
    [SerializeField] private Vector3[] points;
    [SerializeField] private int[] triangles;
    [SerializeField] private float scalar;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    [ContextMenu("Generate Mesh")]
    public void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        points = CreatePoints();
        mesh.SetVertices(points);
        triangles = CreateTriangles();
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    [ContextMenu("Displace")]
    public void DisplacePoints()
    {
        List<Vector3> meshPoints = new List<Vector3>();
        meshFilter.sharedMesh.GetVertices(meshPoints);
        for (int i = 0; i < meshPoints.Count; i++)
        {
            meshPoints[i] = new Vector3(meshPoints[i].x, scalar * (Mathf.PerlinNoise(meshPoints[i].x, meshPoints[i].z) - 0.5f), meshPoints[i].z);
        }
        meshFilter.sharedMesh.SetVertices(meshPoints);
        points = meshPoints.ToArray();
        meshFilter.sharedMesh.RecalculateNormals();
    }

    private Vector3[] CreatePoints()
    {
        Vector3[] result = new Vector3[resolution.x * resolution.y];
        Vector3 start = -(Vector3.right * size.x / 2) + -(Vector3.forward * size.y / 2);

        int count = 0;
        for (int x = 0; x < resolution.x; x++)
        {
            for (int y = 0; y < resolution.y; y++)
            {
                result[count++] = start + (size.x / (resolution.x - 1) * x * Vector3.right) + (size.y / (resolution.y - 1) * y * Vector3.forward);
            }
        }
        return result;
    }

    private int[] CreateTriangles()
    {
        int[] result = new int[(resolution.x - 1) * (resolution.y - 1) * 3 * 2];

        int nextRow = resolution.x;
        int count = 0;
        for (int y = 0; y < resolution.y - 1; y++)
        {
            int yStart = nextRow * y;
            for (int x = 0; x < resolution.x - 1; x++)
            {
                int firstPTop = yStart + x + nextRow;
                int firstPBot = yStart + x;
                int lastPTop = yStart + x + nextRow + 1;
                int lastPBot = yStart + x + 1;

                result[count++] = firstPBot;
                result[count++] = lastPBot;
                result[count++] = lastPTop;

                result[count++] = firstPBot;
                result[count++] = lastPTop;
                result[count++] = firstPTop;
            }
        }
        return result;
    }

    private void OnDrawGizmos()
    {
        Vector3[] results = CreatePoints();
        int[] triangles = CreateTriangles();

        float step = (float)1 / (triangles.Length - 2);
        for (int i = 0; i < triangles.Length - 1; i++)
        {
            Gizmos.color = new Color(i * step, -i * step, -i * step);
            Gizmos.DrawLine(results[triangles[i]], results[triangles[i + 1]]);
        }

        Vector3[] normals = meshFilter.sharedMesh.normals;
        Gizmos.color = Color.green;
        for (int i = 0; i < normals.Length; i++)
        {
            Gizmos.DrawLine(results[i], normals[i] + results[i]);
        }
    }
}