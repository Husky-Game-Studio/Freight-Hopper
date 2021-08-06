using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDirection : MonoBehaviour
{
    [SerializeField]
    PathCreator pathObject;
    Vector3 pathUp = Vector3.up; //Paths & roads currently generate with their surface facing Vector3.up

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
}
