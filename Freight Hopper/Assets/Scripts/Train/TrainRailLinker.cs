using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainRailLinker : MonoBehaviour
{
    [SerializeField] private PIDSettings horizontalControllerSettings;
    [SerializeField] private float followDistance;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float derailThreshold;
    public float FollowDistance => followDistance;
    public Vector3 Offset => offset;
    public float DerailThreshold => derailThreshold;

    private List<int> indexesToRemove = new List<int>();
    private List<TrainData> linkedTrainObjects = new List<TrainData>();

    private PathCreator pathCreator;

    private void Reset()
    {
        followDistance = 10;
        offset = Vector3.up * 5;
        derailThreshold = 25;
    }

    [System.Serializable]
    private class TrainData
    {
        public Rigidbody rb;
        public float startingT;
        public float t;
        public PID controller;
    }

    private void Start()
    {
        pathCreator = GetComponent<PathCreator>();
    }

    // Links rigidbody to the rail, assuming its a cart
    public void Link(Rigidbody rb, float startingT)
    {
        TrainData trainObject = new TrainData
        {
            rb = rb,
            startingT = startingT,
            t = startingT
        };
        PID.Data controllerData = new PID.Data(horizontalControllerSettings);
        trainObject.controller = new PID();
        trainObject.controller.Initialize(controllerData * rb.mass);
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
                trainObject.t = pathCreator.path.NumSegments - 0.01f;
                return;
            }
        }
    }

    // Gets position on path given t, but with the offset considered
    public Vector3 TargetPos(float t)
    {
        return pathCreator.GetPositionOnPath(t) + offset;
    }

    private Vector3 ForwardDirection(TrainData trainObject)
    {
        return (TargetPos(trainObject.t + 0.01f) - TargetPos(trainObject.t)).normalized;
    }

    // Gets error for PID, the error is the horizontal distance from the rail
    private float GetError(TrainData trainObject, Vector3 right)
    {
        Vector3 displacement = TargetPos(trainObject.t) - trainObject.rb.position;
        Vector3 rightDisplacement = Vector3.Project(displacement, right);

        float direction = Mathf.Sign(Vector3.Dot(right, rightDisplacement));

        return direction * rightDisplacement.magnitude;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < linkedTrainObjects.Count; i++)
        {
            AdjustT(linkedTrainObjects[i]);
            if (linkedTrainObjects[i].t == linkedTrainObjects[i].startingT)
            {
                continue;
            }
            if (linkedTrainObjects[i].t + 0.01f > pathCreator.path.NumSegments)
            {
                indexesToRemove.Add(i);
                continue;
            }

            Vector3 right = Vector3.Cross(CustomGravity.GetUpAxis(linkedTrainObjects[i].rb.position), ForwardDirection(linkedTrainObjects[i]));
            float error = GetError(linkedTrainObjects[i], right);
            Vector3 force = linkedTrainObjects[i].controller.GetOutput(error, Time.fixedDeltaTime) * right;
            linkedTrainObjects[i].rb.AddForce(force, ForceMode.Force);
            if ((linkedTrainObjects[i].rb.position - TargetPos(linkedTrainObjects[i].t)).magnitude > derailThreshold)
            {
                indexesToRemove.Add(i);
            }
        }

        for (int i = 0; i < indexesToRemove.Count; i++)
        {
            linkedTrainObjects.RemoveAt(indexesToRemove[0]);
            indexesToRemove.RemoveAt(0);
        }
    }
}