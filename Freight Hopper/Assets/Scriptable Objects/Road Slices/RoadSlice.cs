using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Road Slice")]
public class RoadSlice : ScriptableObject
{
    [SerializeField]
    private Vector3[] points;
    [SerializeField]
    private Vector2Int[] connections;
    [SerializeField]
    private Vector2[] uvs;

    public Vector3[] Points
    {
        get
        {
            return points;
        }
    }

    public Vector2Int[] Connections
    {
        get
        {
            return connections;
        }
    }

    public Vector2[] Uvs
    {
        get
        {
            return uvs;
        }
    }


}
