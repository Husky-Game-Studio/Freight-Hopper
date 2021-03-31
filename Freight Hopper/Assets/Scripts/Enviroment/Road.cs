using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WIP by Jacob Woodley as of March 20, 2021 11:45pm
/// </summary>
[System.Serializable]
public class Road
{
    BezierPath path;
    [SerializeField]
    Vector3[] roadPoints;
    int detail;

    Vector3[] segmentPoints;
    int[] segmentConnections;
    Vector2[] segmentUVs;

    public Vector3[] RoadPoints { get; }
    public Vector3[] SegmentPoints { get; set; }
    public BezierPath Path { get; set; }

    public Road(BezierPath path)
    {
        this.path = path;
    }

    public void ChangeSegmentShape(Vector3[] points, int[] connections, Vector2[] uvs)
    {
        segmentPoints = points;
        segmentConnections = connections;
        segmentUVs = uvs;
    }

    public void UpdateRoadPoints(BezierPath path, int detail)
    {
        this.path = path;
        this.detail = detail;
        roadPoints = new Vector3[detail * path.NumSegments + 1];
        for (int i = 0; i <= detail * path.NumSegments; i++)
        {
            roadPoints[i] = path.GetPathPoint((float)i / detail);
        }
    }

    private bool AllSettingsExist()
    {
        return (path != null && roadPoints != null && segmentPoints != null);
    }

    public Mesh CreateMesh()
    {
        if (AllSettingsExist())
        {
            int pointsPerSegment = segmentPoints.Length;
            Vector3[] vers = new Vector3[pointsPerSegment * (detail * path.NumSegments + 1)];
            Vector2[] uvs = new Vector2[pointsPerSegment * (detail * path.NumSegments + 1)];
            int[] tris = new int[(3 * segmentConnections.Length) * (detail * path.NumSegments)];

            float cummulativeDistance = 0;
            for (int i = 0; i < detail * path.NumSegments + 1; i++)
            {
                if (i > 0)
                {
                    cummulativeDistance += (roadPoints[i] - roadPoints[i - 1]).magnitude;
                }
                Vector3 currentPoint = roadPoints[i];
                Vector3 direciton = GetPathDirection(i);
                SegmentVertices(currentPoint, direciton).CopyTo(vers, pointsPerSegment * i);
                SegmentUVs(cummulativeDistance).CopyTo(uvs, pointsPerSegment * i);
            }
            for (int i = 0; i < detail * path.NumSegments; i++)
            {
                SegmentTris(i).CopyTo(tris, (3 * segmentConnections.Length) * i);
                /*
                int[] theTris = SegmentTris(i);
                Debug.Log("Segment " + i);
                for (int j = 0; j < theTris.Length / 3; j++)
                {
                    Debug.Log("1st: " + theTris[3 * j]);
                    Debug.Log("2nd: " + theTris[3 * j + 1]);
                    Debug.Log("3rd: " + theTris[3 * j + 2]);
                }
                */
            }


            Mesh mesh = new Mesh();
            mesh.vertices = vers;
            mesh.triangles = tris;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }

        else
        {
            throw new System.Exception("Not all settings exist to generate mesh");
        }
    }

    private Vector3 GetPathDirection(int i)
    {
        if (0 < i && i < detail * path.NumSegments)
            return (roadPoints[i + 1] - roadPoints[i - 1]).normalized;
        else if (0 == i)
            return (roadPoints[i + 1] - roadPoints[i]).normalized;
        else
            return (roadPoints[i] - roadPoints[i - 1]).normalized;
    }

    private Vector3[] SegmentVertices(Vector3 position, Vector3 direction)
    {
        Vector3[] vertices = new Vector3[segmentPoints.Length];
        for(int i = 0; i < segmentPoints.Length; i++)
        {
            vertices[i] = position + RotateVector(segmentPoints[i], direction);
        }
        return vertices;
    }

    //TODO Create a version that rotates with respect to an "up axis"
    private Vector3 RotateVector(Vector3 displacement, Vector3 direction)
    {
        Vector3 projection = Vector3.ProjectOnPlane(displacement, Quaternion.LookRotation(direction) * Vector3.up);
        Vector3 projectionDisplacement = displacement - projection;
        Vector3 newProjection = Quaternion.LookRotation(projection) * Quaternion.LookRotation(direction) * Vector3.forward * projection.magnitude;
        return newProjection + projectionDisplacement;
    }

    private Vector2[] SegmentUVs(float cummulativeDistance)
    {
        Vector2[] uvs = new Vector2[segmentUVs.Length];
        for(int i = 0; i < segmentUVs.Length; i++)
        {
            uvs[i] = new Vector2(segmentUVs[i].x, segmentUVs[i].y + cummulativeDistance);
        }
        return uvs;
    }

    private int[] SegmentTris(int segmentNumber)
    {
        int segSize = segmentConnections.Length / 2;
        int baseIndex = segmentPoints.Length * segmentNumber;

        int[] tris = new int[6 * segSize];
        for(int i = 0; i < segSize; i++)
        {
            tris[6 * i] = baseIndex + segmentConnections[2 * i + 1];
            tris[6 * i + 1] = baseIndex + segmentConnections[2 * i];
            tris[6 * i + 2] = baseIndex + segmentConnections[2 * i] + segmentPoints.Length;

            tris[6 * i + 3] = baseIndex + segmentConnections[2 * i + 1];
            tris[6 * i + 4] = baseIndex + segmentConnections[2 * i] + segmentPoints.Length;
            tris[6 * i + 5] = baseIndex + segmentConnections[2 * i + 1] + segmentPoints.Length;
        }
        return tris;
    }

    

    
}
