using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class HoverEngine : MonoBehaviour
{
    [System.NonSerialized] private Rigidbody rb;
    [SerializeField, ReadOnly] private Vector3 direction = Vector3.up;
    [SerializeField, ReadOnly] private PID controller = new PID();
    [SerializeField, ReadOnly] private float targetDistance;
    [SerializeField, ReadOnly] private bool automatic;
    [SerializeField, ReadOnly] private bool firing;
    [System.NonSerialized] private TrainRailLinker currentLinker;
    public bool Firing => firing;
    private int followIndex = 0;
    private float followDistance = 0;

    public void DisableEngine()
    {
        automatic = false;
    }

    public void Initialize(Rigidbody rb, PID.Data data, float targetDistance, bool automatic)
    {
        this.rb = rb;
        controller.Initialize(data * rb.mass);
        this.targetDistance = targetDistance;
        this.automatic = automatic;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (direction * targetDistance));
    }

    public void UpdateCurrentLinker(TrainRailLinker newLinker)
    {
        currentLinker = newLinker;
        currentLinker.RemovedRigidbody += CheckIfCanHover;
        if (newLinker.pathCreator.path.isClosedLoop)
        {
            followIndex = (int)((newLinker.pathCreator.path.CalculateClosestVertexIndex(rb.position) + 1) % newLinker.pathCreator.path.length);
        }
        else
        {
            followIndex = Mathf.Min(newLinker.pathCreator.path.CalculateClosestVertexIndex(this.transform.position) + 1, newLinker.pathCreator.path.LastVertexIndex);
        }

        followDistance = newLinker.FollowDistance;
    }

    private void CheckIfCanHover(Rigidbody unlinkedRigidbody)
    {
        if (unlinkedRigidbody.Equals(rb))
        {
            // Memory leaks?????
            //currentLinker.removedRigidbody -= CheckIfCanHover;
            //Debug.Log("Removed ");
            //Debug.Log("removed " + rb.name);
            currentLinker = null;
        }
    }

    public void Hover(float height, TrainRailLinker railLinker, ref Vector3 position)
    {
        if ((object)railLinker == null)
        {
            firing = false;
            //Debug.Log("no rail linker");
            return;
        }
        firing = true;
        PathCreation.VertexPath path = railLinker.pathCreator.path;

        Vector3 positionOnPath = path.GetPoint(followIndex);
        float distance = Vector3.Distance(positionOnPath, position);
        while (distance <= followDistance && followIndex < path.times.Length - 1)
        {
            followIndex = path.GetNextIndex(followIndex, path.isClosedLoop);
            positionOnPath = path.GetPoint(followIndex);
            distance = Vector3.Distance(positionOnPath, position);
        }

        Vector3 up = path.GetNormal(followIndex);
        float currentHeight = Vector3.Project(positionOnPath - position, up).magnitude;
        float heightDif = height - currentHeight;

        Vector3 force = direction * this.controller.GetOutput(heightDif, Time.fixedDeltaTime);
        rb.AddForceAtPosition(force, position, ForceMode.Force);
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    private void FixedUpdate()
    {
        if (automatic)
        {
            Vector3 position = this.transform.position;
            SetDirection(CustomGravity.GetUpAxis());
            Hover(targetDistance, currentLinker, ref position);
        }
    }
}