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
    public RoadSlice slice;
    [Range(1,100)]
    public int roadDetail;

    public void CreateRoad()
    {
        pathCreator = gameObject.GetComponent<PathCreator>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        road = new Road(pathCreator.path);
    }

    public void UpdateMesh()
    {
        RoadShape(slice);
        road.UpdateRoadPoints(pathCreator.path, roadDetail);
        gameObject.GetComponent<MeshFilter>().mesh = road.CreateMesh();
    }

    private void RoadShape(RoadSlice slice)
    {
        road.ChangeSegmentShape(slice.Points, slice.Connections, slice.Uvs);
    }
}
