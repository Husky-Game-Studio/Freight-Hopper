using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreation.PathCreator))]
public class TrainRailLinker : MonoBehaviour
{
    [SerializeField] private PIDSettings horizontalControllerSettings;
    [SerializeField] private float followDistance;
    [SerializeField] private float height;
    [SerializeField] private float derailThreshold;
    public float FollowDistance => followDistance;
    public float Height => height;
    public float DerailThreshold => derailThreshold;

    private List<int> indexesToRemove = new List<int>();
    private List<TrainData> linkedTrainObjects = new List<TrainData>();

    private PathCreation.PathCreator pathCreator;

    private void Reset()
    {
        followDistance = 10;
        height = 5;
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
        pathCreator = GetComponent<PathCreation.PathCreator>();
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

    // Gets position on path given t, but with the offset considered
    public Vector3 TargetPos(float t)
    {
        return pathCreator.path.GetPointAtTime(t) + (height * pathCreator.path.GetNormal(t));
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
            linkedTrainObjects[i].t = pathCreator.path.GetClosestTimeOnPath(linkedTrainObjects[i].rb.position);
            if (linkedTrainObjects[i].t == linkedTrainObjects[i].startingT)
            {
                continue;
            }
            if (Mathf.Abs(pathCreator.path.length - pathCreator.path.GetClosestDistanceAlongPath(linkedTrainObjects[i].rb.position)) < followDistance)
            {
                indexesToRemove.Add(i);

                continue;
            }
            Vector3 up = pathCreator.path.GetNormal(linkedTrainObjects[i].t);
            Vector3 right = Vector3.Cross(up, pathCreator.path.GetDirection(linkedTrainObjects[i].t));
            Debug.DrawLine(linkedTrainObjects[i].rb.position, linkedTrainObjects[i].rb.position + right * 20, Color.red);
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