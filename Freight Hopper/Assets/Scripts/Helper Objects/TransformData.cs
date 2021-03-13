using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }

    public enum Type
    {
        Position,
        Rotation,
        Scale
    }

    public static TransformData Empty
    {
        get
        {
            return new TransformData(Vector3.zero, Quaternion.identity, Vector3.one);
        }
    }
}