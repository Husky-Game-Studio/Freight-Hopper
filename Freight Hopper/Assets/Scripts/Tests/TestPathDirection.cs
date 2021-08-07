using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathDirection : MonoBehaviour
{
    public PathCreator pathObject;
    public float t;
    public float x;

    void OnValidate()
    {
        Test();
    }

    void Test()
    {
        PathDirection pd = new PathDirection
        {
            pathObject = pathObject
        };
        //XUnitsAway
        float newT = pd.XUnitsAway(t, x);
        Vector3 end = pathObject.GetPositionOnPath(newT);
        transform.position = end;
        //Vector3 start = pathObject.GetPositionOnPath(t);
        //Debug.Log("Linear Distance: " + (end - start).magnitude);
        //Debug.DrawLine(start, end, Color.green);
        //Orientation
        transform.rotation = pd.Rotation(newT);
    }
}
