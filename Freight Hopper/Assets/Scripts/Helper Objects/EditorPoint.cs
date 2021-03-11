using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EditorPoint
{
    public TransformData data;

    public EditorPoint(TransformData data)
    {
        this.data = data;
    }
}
