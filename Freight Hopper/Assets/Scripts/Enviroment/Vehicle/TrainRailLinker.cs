using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    private HashSet<TrainData> dataToRemove = new HashSet<TrainData>();
    private HashSet<TrainData> linkedTrainObjects = new HashSet<TrainData>();
    private HashSet<Rigidbody> isRigidbodyLinked = new HashSet<Rigidbody>();

    public event Action<Rigidbody> RemovedRigidbody;

    public Dictionary<Rigidbody, TrainData> linkedRigidbodyObjects = new Dictionary<Rigidbody, TrainData>();

    [HideInInspector] public PathCreation.PathCreator pathCreator;

    public bool WithinFollowDistance(int pointIndex, Vector3 position)
    {
        return (position - (height * pathCreator.path.GetNormal(pointIndex)) - pathCreator.path.GetPoint(pointIndex)).sqrMagnitude < (this.FollowDistance * this.FollowDistance);
    }

    private void Reset()
    {
        followDistance = 10;
        height = 5;
        derailThreshold = 25;
    }

    [System.Serializable]
    public class TrainData
    {
        public Rigidbody rb;
        public HoverController hoverController;
        public int followIndex;
        public PID controller;
    }

    private void Start()
    {
        InitializePathCreator();
    }

    private void InitializePathCreator()
    {
        if (pathCreator == null)
        {
            pathCreator = GetComponent<PathCreation.PathCreator>();
        }
    }

    // Links rigidbody to the rail, assuming its a train cart. Returns if successful
    public void Link(Rigidbody rb)
    {
        InitializePathCreator();
        if (IsRigidbodyLinked(rb) || rb == null)
        {
            return;
        }

        TrainData trainObject = new TrainData
        {
            rb = rb,
        };
        if (pathCreator.path.isClosedLoop)
        {
            trainObject.followIndex = (int)((pathCreator.path.CalculateClosestVertexIndex(rb.position) + 1) % pathCreator.path.length);
        }
        else
        {
            trainObject.followIndex = (int)Mathf.Min(pathCreator.path.CalculateClosestVertexIndex(rb.position) + 1, pathCreator.path.LastVertexIndex);
            //Debug.Log("Path is open");
        }
        // Could be bad for performance !!
        trainObject.hoverController = trainObject.rb.GetComponentInChildren<HoverController>();
        trainObject.hoverController.LinkEngines(this);
        PID.Data controllerData = new PID.Data(horizontalControllerSettings);
        isRigidbodyLinked.Add(rb);
        trainObject.controller = new PID();
        trainObject.controller.Initialize(controllerData * rb.mass);
        linkedTrainObjects.Add(trainObject);
        linkedRigidbodyObjects[rb] = trainObject;
        return;
    }

    public void RemoveLink(Rigidbody rb)
    {
        if (!IsRigidbodyLinked(rb))
        {
            return;
        }
        TrainData data = linkedRigidbodyObjects[rb];
        linkedTrainObjects.Remove(data);
        linkedRigidbodyObjects.Remove(rb);
        isRigidbodyLinked.Remove(rb);
        RemovedRigidbody?.Invoke(rb);
    }

    public bool IsRigidbodyLinked(Rigidbody rb)
    {
        if (rb == null)
        {
            return false;
        }
        return isRigidbodyLinked.Contains(rb);
    }

    // Gets position on path given t, but with the offset considered
    private Vector3 TargetPos(Vector3 positionOnRail, Vector3 normal)
    {
        return positionOnRail + (height * normal);
    }

    // Gets error for PID, the error is the horizontal distance from the rail
    private float GetError(Vector3 positionOnRail, Vector3 normal, Vector3 trainPosition, Vector3 right)
    {
        Vector3 displacement = TargetPos(positionOnRail, normal) - trainPosition;
        Vector3 rightDisplacement = Vector3.Project(displacement, right);

        float direction = Mathf.Sign(Vector3.Dot(right, rightDisplacement));

        return direction * rightDisplacement.magnitude;
    }

    private void FixedUpdate()
    {
        foreach (TrainData data in linkedTrainObjects)
        {
            if (data.rb == null)
            {
                dataToRemove.Add(data);
                //Debug.Log("Removed destroyed train");
                continue;
            }
            PathCreation.VertexPath path = pathCreator.path;
            int dataFollowIndex = Mathf.Min(data.followIndex, path.LastVertexIndex);
            Vector3 positionOnPath = path.GetPoint(dataFollowIndex);
            Vector3 normal;
            float distance = Vector3.Distance(positionOnPath, data.rb.position);

            while (distance <= followDistance && data.followIndex < path.times.Length - 1)
            {
                data.followIndex = path.GetNextIndex(data.followIndex, path.isClosedLoop);
                positionOnPath = path.GetPoint(data.followIndex);
                distance = Vector3.Distance(positionOnPath, data.rb.position);
            }

            normal = path.GetNormal(dataFollowIndex);
            //Debug.Log("linked to path " + pathCreator.name);
            Debug.DrawLine(positionOnPath, positionOnPath + (normal * 20), Color.green);
            Vector3 right = Vector3.Cross(normal, path.GetTangent(dataFollowIndex));
            //Debug.DrawLine(linkedTrainObjects[i].rb.position, linkedTrainObjects[i].rb.position + right * 20, Color.red);
            float error = GetError(positionOnPath, normal, data.rb.position, right);
            Vector3 force = data.controller.GetOutput(error, Time.fixedDeltaTime) * right;
            data.rb.AddForce(force, ForceMode.Force);
            //float derailDistance = (data.rb.position - TargetPos(positionOnPath, normal)).magnitude;
            if (error > derailThreshold)
            {
                dataToRemove.Add(data);
                //Debug.Log("Removed " + data.rb.name + " because of error of " + derailDistance);
            }
        }

        foreach (TrainData data in dataToRemove)
        {
            RemoveLink(data.rb);
            //isRigidbodyLinked.Remove(data.rb);
            //linkedRigidbodyObjects.Remove(data.rb);
            //RemovedRigidbody?.Invoke(data.rb);
            //linkedTrainObjects.Remove(data);
        }
        dataToRemove.Clear();
    }
}