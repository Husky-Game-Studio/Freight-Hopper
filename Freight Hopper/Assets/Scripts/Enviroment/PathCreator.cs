using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    //Data
    public BezierPath path;

    //Saved Editor Values
    public int focusIndex;

    public void CreatePath()
    {
        path = new BezierPath(transform.position);
    }

    private void OnValidate()
    {
        focusIndex = Mathf.Clamp(focusIndex, 0, path.NumAnchors - 1);
    }

}
