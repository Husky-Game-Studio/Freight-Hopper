using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class HoverEngine : MonoBehaviour
{
    private VisualEffect effect;
    private bool is_effect_playing = false;

    [SerializeField, ReadOnly] private Rigidbody rb;
    [SerializeField, ReadOnly] private Vector3 direction = Vector3.down;
    [SerializeField, ReadOnly] private PID controller = new PID();
    [SerializeField, ReadOnly] private float targetDistance;
    [SerializeField, ReadOnly] private bool automatic;
    [SerializeField, ReadOnly] private bool firing;
    [SerializeField, ReadOnly] private TrainRailLinker currentLinker;
    public bool Firing => firing;
    private int followIndex = 0;
    private float followDistance = 0;

    private void Awake()
    {
        effect = GetComponent<VisualEffect>();
        effect.SetFloat("Downward", -5.0f);
        effect.SetFloat("power", 4.0f);
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
        followIndex = (int)Mathf.Min(newLinker.pathCreator.path.CalculateClosestVertexIndex(this.transform.position) + 1, newLinker.pathCreator.path.length - 1);
        followDistance = newLinker.FollowDistance;
    }

    private void CheckIfCanHover(Rigidbody unlinkedRigidbody)
    {
        if (unlinkedRigidbody.Equals(rb))
        {
            // Memory leaks?????
            //currentLinker.removedRigidbody -= CheckIfCanHover;
            //Debug.Log("Removed " + unlinkedRigidbody.name, this.gameObject);
            currentLinker = null;
        }
    }

    private void AddForce(float heightDifference)
    {
        float error = heightDifference;
        Debug.DrawLine(this.transform.position, this.transform.position + (-direction * error), Color.white);
        // We don't want the hover engine to correct itself downwards. Hovering only applys upwards!
        if (error > -0.1f)
        {
            Vector3 force = -direction * this.controller.GetOutput(error, Time.fixedDeltaTime);

            rb.AddForceAtPosition(force, this.transform.position, ForceMode.Force);
        }
    }

    public void Hover(float height, TrainRailLinker path)
    {
        if (path == null)
        {
            firing = false;
            return;
        }
        firing = true;

        Vector3 positionOnPath = path.pathCreator.path.GetPoint(followIndex);
        Vector3 normal;
        float distance = Vector3.Distance(positionOnPath, this.transform.position);
        // distance <= followDistance && linkedTrainObjects[i].followIndex < pathCreator.path.times.Length - 1
        while (distance <= followDistance && followIndex < path.pathCreator.path.times.Length - 1)
        {
            //Debug.Log("distance between position on path and current is " + distance + " and follow distance is " + followDistance);
            followIndex = path.pathCreator.path.GetNextIndex(followIndex);
            positionOnPath = path.pathCreator.path.GetPoint(followIndex);
            distance = Vector3.Distance(positionOnPath, this.transform.position);
        }
        normal = path.pathCreator.path.GetNormal(followIndex);
        Debug.DrawLine(positionOnPath, positionOnPath + (normal * 20), Color.green);
        Debug.DrawLine(positionOnPath, this.transform.position, Color.red);
        AddForce(height - Vector3.Project(positionOnPath - this.transform.position, normal).magnitude);
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    private void FixedUpdate()
    {
        if (automatic)
        {
            SetDirection(-CustomGravity.GetUpAxis(this.transform.position));
            Hover(targetDistance, currentLinker);
        }
        if (firing)
        {
            effect.SetVector3("spawn", this.transform.position);
            if (!is_effect_playing)
            {
                effect.Play();
                is_effect_playing = true;
            }
        }
        else if (!firing && is_effect_playing)
        {
            effect.Stop();
            is_effect_playing = false;
        }
    }
}