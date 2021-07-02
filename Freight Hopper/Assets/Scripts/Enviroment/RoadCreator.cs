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
        return pathCreator.GetPositionOnPath(t);
    }
    public float FindClosestT(Vector3 currentPosition)
    {
        float test_t = 0.5f * pathCreator.GetPathSegmentCount();
        float dt = 0.25f * pathCreator.GetPathSegmentCount();
        for (int i = 0; i < 8; i++) //2^8 possible points to select on the path
        {
            float d1 = (GetPositionOnPath(test_t + dt) - currentPosition).sqrMagnitude;
            float d2 = (GetPositionOnPath(test_t - dt) - currentPosition).sqrMagnitude;
            test_t += (d1 < d2) ? dt : -dt;
            dt /= 2;
        }
        return test_t;
    }

    public void UpdateMesh()
    {
        RoadShape(slice);
        if (GetComponent<MeshCollider>() != null)
        {
            DestroyImmediate(GetComponent<MeshCollider>());
            this.gameObject.AddComponent<MeshCollider>();
        }
        road.UpdateRoadPoints(pathCreator.path, roadDetail);
        this.gameObject.GetComponent<MeshFilter>().mesh = road.CreateMesh();
    }

    private void RoadShape(RoadSlice slice)
    {
        Vector3[] points = slice.Points();

        for (int i = 0; i < points.Length; i++)
        {
            if (slice.RailSize.Enabled)
            {
                points[i] /= slice.RailSize.value;
            }
            if (slice.RailSeperationDistance.Enabled)
            {
                float seperationValue = slice.RailSeperationDistance.value / 2;
                if (i < points.Length / 2)
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

        road.ChangeSegmentShape(points, slice.Connections(), slice.Uvs());
    }
}