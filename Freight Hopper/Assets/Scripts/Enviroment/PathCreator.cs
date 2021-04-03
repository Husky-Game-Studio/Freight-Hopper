using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    [HideInInspector] //The BezierPath should not be modified directly as some constraints must be met
    public BezierPath path; //The data
    public int focusIndex; //Saved editor value

    public void CreatePath()
    {
        path = new BezierPath();
    }
}
