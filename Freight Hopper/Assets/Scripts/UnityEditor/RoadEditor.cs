using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(RoadCreator))]
public class RoadEditor : Editor
{
    private RoadCreator creator;

    private void OnEnable()
    {
        creator = (RoadCreator)this.target;
        if (creator.pathCreator == null)
        {
            creator.CreateRoad();
        }
    }

    public override void OnInspectorGUI()
    {
        if (creator == null)
            return;

        if (GUILayout.Button("Generate Mesh WIP"))
        {
            creator.UpdateMesh();
        }

        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        if (creator == null)
            return;
        RoadPoints();
    }

    private void RoadPoints()
    {
        if (creator.road.RoadPoints != null)
        {
            Vector3[] roadPoints = creator.road.RoadPoints;
            for (int i = 0; i < creator.road.RoadPoints.Length; i++)
            {
                Handles.SphereHandleCap(0, creator.transform.TransformPoint(roadPoints[i]), Quaternion.identity, 0.2f, EventType.Repaint);
            }
        }
    }
}

#endif