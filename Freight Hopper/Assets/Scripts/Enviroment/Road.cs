using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WIP by Jacob Woodley as of March 20, 2021 12:50am
/// </summary>
public class Road
{
    BezierPath path;
    Vector3[] roadPoints;
    int detail;

    Vector3[] segmentPoints;

    public Vector3[] SegmentPoints { get; set; }
    public BezierPath Path { get; set; }

    public Road(BezierPath path)
    {
        this.path = path;
    }

    public void UpdateRoadPoints(BezierPath path, int detail)
    {
        this.path = path;
        this.detail = detail;
        roadPoints = new Vector3[detail * path.NumAnchors];
        for (int i = 0; i < detail * path.NumAnchors; i++)
        {
            roadPoints[i] = path.GetPathPoint((float)i / detail);
        }
    }

    /*
    public Mesh CreateMesh()
    {
        if (AllSettingsExist())
        {
            int pointsPerSegment = segmentPoints.Length;
            Vector3[] vers = new Vector3[pointsPerSegment * detail * path.NumAnchors];
            Vector2[] uvs = new Vector2[pointsPerSegment * detail * path.NumAnchors];
            int[] tri = new int[6 * detail * path.NumSegments];

            float cummulativeDistance = 0;
            for (int i = 0; i < detail * path.NumAnchors; i++)
            {
                Vector3 currentPoint = roadPoints[i];
                Vector3 direciton = GetPathDirection(i);
                if (i > 0)
                {
                    cummulativeDistance += (roadPoints[i] - roadPoints[i - 1]).magnitude;
                }
            }
        }
        else
        {
            throw new System.Exception("Not all settings exist to generate mesh? Forgetting ..."); //TODO update error message
        }

        Vector3.RotateTowards()
    }
    */

    private Vector3 GetPathDirection(int i)
    {
        if (0 < i && i < detail * path.NumAnchors - 1)
            return (roadPoints[i + 1] - roadPoints[i - 1]).normalized;
        else if (0 == i)
            return (roadPoints[i + 1] - roadPoints[i - 0]).normalized;
        else
            return (roadPoints[i] - roadPoints[i - 1]).normalized;
    }

    private bool AllSettingsExist()
    {
        return (path != null && roadPoints != null && segmentPoints != null);
    }
}
