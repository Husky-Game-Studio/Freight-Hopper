using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainRailLinker : MonoBehaviour
{
    [SerializeField] private PID.Data horizontalControllerSettings;
    [SerializeField] private float followDistance;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float derailThreshold;
    private List<int> indexesToRemove = new List<int>();
    [SerializeField, ReadOnly] private List<TrainData> linkedTrainObjects = new List<TrainData>();

    private PathCreator pathCreator;

    [System.Serializable]
    private class TrainData
    {
        public Rigidbody rb;
        public float t;
        public PID controller;
    }

    private void Start()
    {
        pathCreator = GetComponent<PathCreator>();
    }

    public void Link(Rigidbody rb)
    {
        TrainData trainObject = new TrainData();
        trainObject.rb = rb;
        trainObject.t = 0;
        trainObject.controller = new PID();
        trainObject.controller.Initialize(horizontalControllerSettings * rb.mass);
        linkedTrainObjects.Add(trainObject);
    }

    private void AdjustT(TrainData trainObject)
    {
        Debug.DrawLine(trainObject.rb.position, TargetPos(trainObject.t));
        while ((TargetPos(trainObject.t) - trainObject.rb.transform.position).magnitude < followDistance)
        {
            trainObject.t += 0.01f;
            if (trainObject.t >= pathCreator.path.NumSegments)
            {
                trainObject.t = pathCreator.path.NumSegments;
                return;
            }
        }
    }

    public Vector3 TargetPos(float t)
    {
        return pathCreator.GetPositionOnPath(t) + offset;
    }

    private Vector3 ForwardDirection(TrainData trainObject)
    {
        return (TargetPos(trainObject.t + 0.01f) - TargetPos(trainObject.t)).normalized;
    }

    private float GetError(TrainData trainObject, Vector3 forward, Vector3 right)
    {
        Vector3 displacement = TargetPos(trainObject.t) - trainObject.rb.position;
        Vector3 rightDisplacement = Vector3.Project(displacement, right);

        float scalar = 0;
        if (rightDisplacement.x != 0)
        {
            scalar = right.x / rightDisplacement.x;
        }
        else if (rightDisplacement.y != 0)
        {
            scalar = right.y / rightDisplacement.y;
        }
        else
        {
            scalar = right.z / rightDisplacement.z;
        }
        float direction = Mathf.Sign(scalar);
        if (direction < 0)
        {
            Debug.DrawLine(rightDisplacement + trainObject.rb.position, trainObject.rb.position, Color.blue);
        }
        else
        {
            Debug.DrawLine(rightDisplacement + trainObject.rb.position, trainObject.rb.position, Color.red);
        }

        return direction * rightDisplacement.magnitude;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < linkedTrainObjects.Count; i++)
        {
            AdjustT(linkedTrainObjects[i]);
            if (linkedTrainObjects[i].t == 0)
            {
                continue;
            }
            if (linkedTrainObjects[i].t + 0.01f >= pathCreator.path.NumSegments)
            {
                indexesToRemove.Add(i);
                continue;
            }

            Vector3 forward = ForwardDirection(linkedTrainObjects[i]);
            Debug.DrawLine(linkedTrainObjects[i].rb.position, linkedTrainObjects[i].rb.position + forward, Color.blue);
            Vector3 right = Vector3.Cross(CustomGravity.GetUpAxis(linkedTrainObjects[i].rb.position), forward);
            Debug.DrawLine(linkedTrainObjects[i].rb.position, linkedTrainObjects[i].rb.position + right, Color.green);
            float error = GetError(linkedTrainObjects[i], forward, right);
            Vector3 force = linkedTrainObjects[i].controller.GetOutput(error, Time.fixedDeltaTime) * right;
            linkedTrainObjects[i].rb.AddForce(force, ForceMode.Force);
            if ((linkedTrainObjects[i].rb.position - TargetPos(linkedTrainObjects[i].t)).magnitude > derailThreshold)
            {
                indexesToRemove.Add(i);
            }
        }

        foreach (int i in indexesToRemove)
        {
            linkedTrainObjects.RemoveAt(i);
        }

        indexesToRemove.Clear();
    }
}