using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PointCollection : MonoBehaviour
{
    public List<EditorPoint> points = new List<EditorPoint>();

    public virtual void Add()
    {
        points.Add(new EditorPoint(TransformData.empty));
    }


    public abstract void Add(TransformData data);

    public virtual bool RemoveAt(int index)
    {
        if (index >= 0 && index <= points.Count - 1)
        {
            points.RemoveAt(index);
            return true;
        }
        return false;
    }

    public abstract bool Remove(TransformData data);

    public virtual bool RemoveEnd()
    {
        return RemoveAt(points.Count - 1);
    }

    public virtual bool RemoveStart()
    {
        return RemoveAt(0);
    }

    public EditorPoint this[int index]
    {
        get
        {
            return points[index];
        }
        set
        {
            points[index] = value;
        }
    }
}
