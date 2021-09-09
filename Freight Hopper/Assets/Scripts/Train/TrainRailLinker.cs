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
    private HashSet<Rigidbody> isRigidbodyLinked = new HashSet<Rigidbody>();
    public Dictionary<Rigidbody, TrainData> linkedRigidbodyObjects = new Dictionary<Rigidbody, TrainData>();

    [HideInInspector] public PathCreation.PathCreator pathCreator;

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
    public bool Link(Rigidbody rb)
    {
        InitializePathCreator();
        if (IsRigidbodyLinked(rb))
        {
            return false;
        }

        TrainData trainObject = new TrainData
        {
            rb = rb,
            followIndex = pathCreator.path.GetClosestVertexTimeIndex(rb.position),
        };
        PID.Data controllerData = new PID.Data(horizontalControllerSettings);
        isRigidbodyLinked.Add(rb);
        trainObject.controller = new PID();
        trainObject.controller.Initialize(controllerData * rb.mass);
        linkedTrainObjects.Add(trainObject);
        linkedRigidbodyObjects[rb] = trainObject;
        return true;
    }

    public bool IsRigidbodyLinked(Rigidbody rb)
    {
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
        Debug.DrawLine(positionOnRail, positionOnRail + rightDisplacement, Color.yellow);

        float direction = Mathf.Sign(Vector3.Dot(right, rightDisplacement));

        return direction * rightDisplacement.magnitude;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < linkedTrainObjects.Count; i++)
        {
            if (linkedTrainObjects[i].rb == null)
            {
                indexesToRemove.Add(i);
                Debug.Log("Removed destroyed train");
                continue;
            }
            if (pathCreator.path.cumulativeLengthAtEachVertex[linkedTrainObjects[i].followIndex] >= pathCreator.path.length - followDistance
                && Vector3.Distance(linkedTrainObjects[i].rb.position, pathCreator.path.GetPoint(linkedTrainObjects[i].followIndex)) <= followDistance)
            {
                indexesToRemove.Add(i);
                Debug.Log("Removed " + linkedTrainObjects[i].rb.name + " because of it reached the end");
                continue;
            }

            Vector3 positionOnPath = pathCreator.path.GetPoint(linkedTrainObjects[i].followIndex);
            Vector3 normal;
            float distance = Vector3.Distance(positionOnPath, linkedTrainObjects[i].rb.position);
            while (distance <= followDistance && linkedTrainObjects[i].followIndex < pathCreator.path.times.Length - 1)
            {
                //Debug.Log("distance between position on path and current is " + distance + " and follow distance is " + followDistance);
                linkedTrainObjects[i].followIndex = pathCreator.path.GetNextIndex(linkedTrainObjects[i].followIndex);
                positionOnPath = pathCreator.path.GetPoint(linkedTrainObjects[i].followIndex);
                distance = Vector3.Distance(positionOnPath, linkedTrainObjects[i].rb.position);
            }

            normal = pathCreator.path.GetNormal(linkedTrainObjects[i].followIndex);
            Debug.DrawLine(positionOnPath, positionOnPath + (normal * 20), Color.green);
            Vector3 right = Vector3.Cross(normal, pathCreator.path.GetDirection(pathCreator.path.times[linkedTrainObjects[i].followIndex]));
            //Debug.DrawLine(linkedTrainObjects[i].rb.position, linkedTrainObjects[i].rb.position + right * 20, Color.red);
            float error = GetError(positionOnPath, normal, linkedTrainObjects[i].rb.position, right);
            Vector3 force = linkedTrainObjects[i].controller.GetOutput(error, Time.fixedDeltaTime) * right;
            linkedTrainObjects[i].rb.AddForce(force, ForceMode.Force);
            float derailDistance = (linkedTrainObjects[i].rb.position - TargetPos(positionOnPath, normal)).magnitude;
            if (error > derailThreshold)
            {
                indexesToRemove.Add(i);
                Debug.Log("Removed " + linkedTrainObjects[i].rb.name + " because of error of " + derailDistance);
            }
        }

        for (int i = 0; i < indexesToRemove.Count; i++)
        {
            isRigidbodyLinked.Remove(linkedTrainObjects[indexesToRemove[0]].rb);
            linkedRigidbodyObjects[linkedTrainObjects[indexesToRemove[0]].rb] = null;
            linkedTrainObjects.RemoveAt(indexesToRemove[0]);

            indexesToRemove.RemoveAt(0);
        }
    }
}