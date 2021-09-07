using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailConverter : MonoBehaviour
{
    public PathCreator[] oldPathCreators;
    public RoadCreator[] allRoadCreators;

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
    }

    public void Convert(PathCreator creator)
    {
        PathCreation.PathCreator newNewPathCreator =
        creator.gameObject.AddComponent<PathCreation.PathCreator>();
        creator.gameObject.AddComponent<PathCreation.Examples.CylinderMeshCreator>();
        newNewPathCreator.InitializeEditorData(false);
        Debug.Log("is newpath creator null? " + newNewPathCreator == null);
        for (int i = 0; i < creator.path.NumSegments; i++)
        {
            newNewPathCreator.editorData._bezierPath.AddSegmentToEnd(creator.path.GetSegment(i).a);
        }
        for (int i = 0; i < creator.path.points.Count; i++)
        {
            newNewPathCreator.editorData._bezierPath.SetPoint(i, creator.path.points[i], true);
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
}