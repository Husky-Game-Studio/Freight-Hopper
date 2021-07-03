using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    [HideInInspector] //The BezierPath should not be modified directly as some constraints must be met
    public BezierPath path; //The data

    public int focusIndex; //Saved editor value

    public Vector3 GetPositionOnPath(float t)
    {
        return this.transform.TransformPoint(path.GetPathPoint(t));
    }

    public float GetPathSegmentCount()
    {
        return path.NumSegments;
    }

    [ContextMenu("Reverse Path")]
    public void ReversePathOrder()
    {
        path.ReversePoints();
    }

    public void CreatePath()
    {
        path = new BezierPath();
    }
}