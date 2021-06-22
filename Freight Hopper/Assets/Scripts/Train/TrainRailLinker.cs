using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainRailLinker : MonoBehaviour
{
    [SerializeField] private PID.Data horizontalControllerSettings;
    private float followDistance;
    [SerializeField, ReadOnly] private List<TrainData> linkedTrainObjects;

    private PathCreator pathCreator;

    private class TrainData
    {
        public Rigidbody rb;
        public float t;
        public PID controller;
    }

    public void Link(Rigidbody rb)
    {
        TrainData trainObject = new TrainData();
        trainObject.rb = rb;
        trainObject.t = 0;
        trainObject.controller = new PID();
        trainObject.controller.Initialize(horizontalControllerSettings);
    }

    public void Unlink(Rigidbody rb)
    {
        for (int i = 0; i < linkedTrainObjects.Count; i++)
        {
            if (linkedTrainObjects[i].rb == rb)
            {
                linkedTrainObjects.RemoveAt(i);
                break;
            }
        }
    }

    private void AdjustT(TrainData trainObject)
    {
        while ((pathCreator.GetPositionOnPath(trainObject.t) - trainObject.rb.transform.position).magnitude < followDistance)
        {
            trainObject.t += 0.01f;
            if (trainObject.t >= pathCreator.path.NumSegments)
            {
                trainObject.t = pathCreator.path.NumSegments;
                return;
            }
        }
    }

    private void FixedUpdate()
    {
    }
}