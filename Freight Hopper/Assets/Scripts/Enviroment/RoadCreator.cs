using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RoadCreator : MonoBehaviour
{
    public PathCreator pathCreator;

    public Road road;
    private MeshFilter meshFilter;
    public RoadSlice slice;

    [Range(1, 100)]
    public int roadDetail;

    public void CreateRoad()
    {
        if (pathCreator == null)
        {
            pathCreator = this.gameObject.GetComponent<PathCreator>();
            if (pathCreator == null)
            {
                Debug.Log("No path creator found, please input one or add one as a component");
            }
        }

        meshFilter = this.gameObject.GetComponent<MeshFilter>();
        road = new Road(pathCreator.path);
    }

    public Vector3 GetPositionOnPath(float t)
    {
        return transform.TransformPoint(pathCreator.path.GetPathPoint(t));
    }

    public void UpdateMesh()
    {
        RoadShape(slice);
        road.UpdateRoadPoints(pathCreator.path, roadDetail);
        this.gameObject.GetComponent<MeshFilter>().mesh = road.CreateMesh();
    }

    private void RoadShape(RoadSlice slice)
    {
        Vector3[] points = new Vector3[slice.Points.Length];
        for (int i = 0; i < slice.Points.Length; i++)
        {
            if (!slice.RailSeperationDistance.Enabled && slice.RailSize.Enabled)
            {
                points[i] = slice.Points[i] / slice.RailSize.value;
            }
            else if (slice.RailSeperationDistance.Enabled && !slice.RailSize.Enabled)
            {
                float seperationValue = slice.RailSeperationDistance.value / 2;
                if (i < slice.Points.Length / 2)
                {
                    points[i] = new Vector3(slice.Points[i].x + seperationValue, slice.Points[i].y, slice.Points[i].z);
                }
                else
                {
                    points[i] = new Vector3(slice.Points[i].x - seperationValue, slice.Points[i].y, slice.Points[i].z);
                }
            }
            else if (slice.RailSeperationDistance.Enabled && slice.RailSize.Enabled)
            {
                float seperationValue = slice.RailSeperationDistance.value / 2;
                points[i] = slice.Points[i] / slice.RailSize.value;
                if (i < slice.Points.Length / 2)
                {
                    points[i] = new Vector3(points[i].x + seperationValue, points[i].y, points[i].z);
                }
                else
                {
                    points[i] = new Vector3(points[i].x - seperationValue, points[i].y, points[i].z);
                }
            }
            else
            {
                points = slice.Points;
                break;
            }
        }
        road.ChangeSegmentShape(points, slice.Connections, slice.Uvs);
    }
}