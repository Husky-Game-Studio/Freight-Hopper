using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailConverter : MonoBehaviour
{
    public PathCreator[] oldPathCreators;
    public RoadCreator[] allRoadCreators;
    public Material railMaterial;

    [ContextMenu("Find all path creators")]
    public void FindAll()
    {
        oldPathCreators = FindObjectsOfType<PathCreator>();
        allRoadCreators = FindObjectsOfType<RoadCreator>();
    }

    [ContextMenu("Convert All")]
    public void ConvertAll()
    {
        for (int i = 0; i < oldPathCreators.Length; i++)
        {
            Convert(oldPathCreators[i]);
        }

        for (int i = 0; i < allRoadCreators.Length; i++)
        {
            DestroyImmediate(allRoadCreators[i]);
        }
        for (int i = 0; i < oldPathCreators.Length; i++)
        {
            DestroyImmediate(oldPathCreators[i]);
        }
    }

    public void Convert(PathCreator creator)
    {
        PathCreation.PathCreator newNewPathCreator =
        creator.gameObject.AddComponent<PathCreation.PathCreator>();
        PathCreation.Examples.CylinderMeshCreator cylinderSettings = creator.gameObject.AddComponent<PathCreation.Examples.CylinderMeshCreator>();
        cylinderSettings.resolutionU = 7;
        cylinderSettings.resolutionV = 5;
        cylinderSettings.thickness = 0.3f;
        cylinderSettings.autoUpdate = false;
        cylinderSettings.meshHolder = new GameObject("Mesh Holder");
        cylinderSettings.meshHolder.transform.SetParent(creator.gameObject.transform);
        if (railMaterial != null)
        {
            cylinderSettings.material = railMaterial;
        }
        newNewPathCreator.bezierPath = new PathCreation.BezierPath(creator.path.points);
        cylinderSettings.TriggerUpdate();
    }
}