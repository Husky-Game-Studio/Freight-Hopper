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

    [SerializeField] private bool automaticlyRemoveAndAddMeshCollider;
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
        return pathCreator.GetPositionOnPath(t);
    }

    public void UpdateMesh()
    {
        RoadShape(slice);
        if (automaticlyRemoveAndAddMeshCollider && GetComponent<MeshCollider>() != null)
        {
            DestroyImmediate(this.GetComponent<MeshCollider>());
            this.gameObject.AddComponent<MeshCollider>();
        }
        road.UpdateRoadPoints(pathCreator.path, roadDetail);
        this.gameObject.GetComponent<MeshFilter>().mesh = road.CreateMesh();
    }

    private void RoadShape(RoadSlice slice)
    {
        Vector3[] points = new Vector3[slice.Points.Length];

        for (int i = 0; i < slice.Points.Length; i++)
        {
            points[i] = slice.Points[i];
        }

        for (int i = 0; i < slice.Points.Length; i++)
        {
            if (slice.RailSize.Enabled)
            {
                points[i] /= slice.RailSize.value;
            }
            if (slice.RailSeperationDistance.Enabled)
            {
                float seperationValue = slice.RailSeperationDistance.value / 2;
                if (i < slice.Points.Length / 2)
                {
                    points[i] += Vector3.right * seperationValue;
                }
                else
                {
                    points[i] -= Vector3.right * seperationValue;
                }
            }

            if (!slice.RailSeperationDistance.Enabled && !slice.RailSize.Enabled)
            {
                break;
            }
        }

        road.ChangeSegmentShape(points, slice.Connections, slice.Uvs);
    }
}