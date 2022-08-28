using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static void DestroyImmediateChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public static void DestroyImmediateChildren(this Transform transform, List<GameObject> exceptions)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            bool skip = false;
            for (int j = 0; j < exceptions.Count; j++)
            {
                if (exceptions[j] == transform.GetChild(i).gameObject)
                {
                    skip = true;
                    break;
                }
            }
            if (skip)
            {
                continue;
            }
            Object.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}