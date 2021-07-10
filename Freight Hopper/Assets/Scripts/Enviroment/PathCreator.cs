using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Monobehavior (added as component) that holds BezierPath data that is visualized using PathEditor.cs
/// </summary>
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