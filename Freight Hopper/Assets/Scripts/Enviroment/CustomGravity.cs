using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomGravity
{
    public static Vector3 GetGravity(Vector3 position)
    {
        return Physics.gravity;
    }

    public static Vector3 GetUpAxis(Vector3 position)
    {
        return -Physics.gravity.normalized;
    }
}