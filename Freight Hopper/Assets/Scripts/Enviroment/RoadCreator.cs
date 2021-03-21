using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(MeshFilter))]
public class RoadCreator : MonoBehaviour
{
    public PathCreator pathCreator;
    
    public Road road;
    private MeshFilter meshFilter;

    public void CreateRoad()
    {
        pathCreator = gameObject.GetComponent<PathCreator>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        road = new Road(pathCreator.path);
        RoadDefaultShape();
    }

    public void UpdateMesh()
    {
        road.UpdateRoadPoints(pathCreator.path, 2);
        meshFilter.mesh = road.CreateMesh();
    }

    private void RoadDefaultShape()
    {
        Vector3[] points = new Vector3[]
        {
            new Vector3(1,0,0),
            new Vector3(1,-1,0),
            new Vector3(-1,-1,0),
            new Vector3(-1,0,0)
        };
        int[] connections = new int[]
        {
            0, 1,
            1, 2,
            2, 3,
            3, 0
        };
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(1,0),
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(0,0)
        };
        road.ChangeSegmentShape(points, connections, uvs);
    }
}
