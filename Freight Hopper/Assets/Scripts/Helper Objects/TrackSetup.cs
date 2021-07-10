using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An addable component that immediately creates the gameobject setup for a train road and deletes itself
/// </summary>
[ExecuteInEditMode]
public class TrackSetup : MonoBehaviour
{
    private void OnEnable()
    {
        Setup();
    }

    void Setup()
    {
        AddParentComponents();
        CreateVisual();
        CreateTrainCollider();
        DestroyImmediate(this);
    }

    void AddParentComponents()
    {
        if (gameObject.GetComponent<PathCreator>() == null)
            gameObject.AddComponent<PathCreator>();
        if (gameObject.GetComponent<TrainRailLinker>() == null)
            gameObject.AddComponent<TrainRailLinker>();
    }

    void CreateVisual()
    {
        GameObject obj = new GameObject();
        obj.transform.parent = transform;
        obj.name = "Visual";
        obj.AddComponent<RoadCreator>();
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<MeshCollider>();
        obj.layer = 0; //Default
    }

    void CreateTrainCollider()
    {
        GameObject obj = new GameObject();
        obj.transform.parent = transform;
        obj.name = "Train Collider";
        obj.AddComponent<RoadCreator>();
        obj.AddComponent<MeshCollider>();
        obj.layer = 12; //IgnorePlayer
    }
}
