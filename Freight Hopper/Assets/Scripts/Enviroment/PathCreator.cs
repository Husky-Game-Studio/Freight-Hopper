using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    [HideInInspector] //The BezierPath should not be modified directly as some constraints must be met
    public BezierPath path; //The data

    public int focusIndex; //Saved editor value
    [Range(0.01f, 0.1f)]
    public float pointSize = 0.05f;

    public Vector3 GetPositionOnPath(float t)
    {
        return this.transform.TransformPoint(path.GetPathPoint(t));
    }

    public Vector3 GetDeltaPositionOnPath(float t)
    {
        return this.transform.TransformVector(path.GetPathDeltaPoint(t));
    }

    public float GetPathSegmentCount()
    {
        return path.NumSegments;
    }

    public void ReversePathOrder()
    {
        path.ReversePoints();
    }

    public void CreatePath()
    {
        path = new BezierPath();
    }
}