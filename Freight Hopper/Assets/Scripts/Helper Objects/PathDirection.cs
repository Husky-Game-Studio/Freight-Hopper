using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDirection
{
    public PathCreator pathObject;
    private Vector3 pathUp = Vector3.up; //Paths & roads currently generate with their surface facing Vector3.up

    public Vector3 Forward(float t)
    {
        return pathObject.GetDeltaPositionOnPath(t).normalized;
    }

    public Vector3 Right(float t)
    {
        return Vector3.Cross(Forward(t), pathUp);
    }

    public Vector3 Up(float t)
    {
        return Vector3.Cross(Right(t), Forward(t));
    }

    public Quaternion Rotation(float t)
    {
        return Quaternion.LookRotation(Forward(t), Up(t));
    }

    /// <summary>
    /// Finds a path t value that corresponds to a point x distance (~arclength) away from the provided starting t value
    /// </summary>
    /// <param name="t"> Path location to start on</param>
    /// <param name="x">+X forward, -X backward, physical units along the path</param>
    /// <returns></returns>
    public float XUnitsAway(float t, float x)
    {
        //Approach: Use the instantaneous cubic bezier velocity to predict the t value worth x physical units away. Repeat from new t value to improve accuracy
        int precision = 10;
        for (int i = precision; i > 0; i--)
        {
            Vector3 currentPoint = pathObject.GetPositionOnPath(t);
            float speed = pathObject.GetDeltaPositionOnPath(t).magnitude;
            float increase = x / speed / i; // x/speed estimates how far x units is in terms of the t variable, i makes the steps towards the prediction smaller (for higher precision values) to minimize error between true arclength and linear distance
            ChangeT(ref t, increase, out bool enteredNewSegment);
            //Update variables to assist in next iteration
            Vector3 nextPoint = pathObject.GetPositionOnPath(t);
            x -= Mathf.Sign(x) * (nextPoint - currentPoint).magnitude; //Adjust next target distance by how much closer the estimate has come along the path
            if (enteredNewSegment && x < 0)
                t -= 0.001f; //Ensure the next iteration of refining the prediction is on the correct segment
        }
        return t;
    }

    private void ChangeT(ref float t, float deltaT, out bool enteredNewSegment)
    {
        if ((int)t == (int)(t + deltaT)) //If the prediction is on same path segment
        {
            enteredNewSegment = false;
            t += deltaT; //Change t assumming constant speed
        }
        else //the prediction has emerged onto a new cubic bezier segment not accounted for
        {
            enteredNewSegment = true;
            if (deltaT > 0)
                t = (int)t + 1; //End of this segment
            else
                t = (int)t; //Beginning of this segment
        }
    }
}