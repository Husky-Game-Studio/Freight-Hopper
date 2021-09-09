using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class HoverEngine : MonoBehaviour
{
    private VisualEffect effect;
    private bool is_effect_playing = false;

    [SerializeField, ReadOnly] private Rigidbody rb;
    [SerializeField, ReadOnly] private Vector3 direction = Vector3.down;
    [SerializeField, ReadOnly] private LayerMask layerMask;
    [SerializeField, ReadOnly] private PID controller = new PID();
    [SerializeField, ReadOnly] private float targetDistance;
    [SerializeField, ReadOnly] private bool automatic;
    [SerializeField, ReadOnly] private bool firing;
    [SerializeField, ReadOnly] private TrainRailLinker currentLinker;
    private Memory<Vector3> position;
    public bool Firing => firing;
    private int followIndex = 0;
    private float followDistance = 0;

    private void Awake()
    {
        effect = GetComponent<VisualEffect>();
        effect.SetFloat("Downward", -5.0f);
        effect.SetFloat("power", 4.0f);
    }

    public void Initialize(Rigidbody rb, LayerMask layerMask, PID.Data data, float targetDistance, bool automatic)
    {
        this.rb = rb;
        controller.Initialize(data * rb.mass);
        this.layerMask = layerMask;
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
        followIndex = newLinker.pathCreator.path.GetClosestVertexTimeIndex(position.current);
        followDistance = newLinker.FollowDistance;
    }

    public void Hover(float height)
    {
        if (Physics.Raycast(position.current, direction, out RaycastHit hit, height + 0.1f, layerMask))
        {
            firing = true;
            AddForce(height - hit.distance);
        }
        else
        {
            firing = false;
        }
    }

    private void AddForce(float heightDifference)
    {
        float error = heightDifference;
        //Debug.DrawLine(position.current, position.current + (-direction * error), Color.white);
        // We don't want the hover engine to correct itself downwards. Hovering only applys upwards!
        if (error > -0.1f)
        {
            Vector3 force = -direction * this.controller.GetOutput(error, Time.fixedDeltaTime);

            rb.AddForceAtPosition(force, position.current, ForceMode.Force);
        }
    }

    public void Hover(float height, TrainRailLinker path)
    {
        if (path == null || !path.IsRigidbodyLinked(rb))
        {
            firing = false;
            return;
        }
        firing = true;

        float time = path.pathCreator.path.GetTimeByIndexAndNextIndex(followIndex);
        Vector3 positionOnPath = path.pathCreator.path.GetPointAtTime(time);
        Vector3 normal;
        float distance = Vector3.Distance(positionOnPath, position.current);
        while (distance <= followDistance && followIndex < path.pathCreator.path.times.Length - 1)
        {
            //Debug.Log("distance between position on path and current is " + distance + " and follow distance is " + followDistance);
            followIndex = path.pathCreator.path.GetNextIndex(followIndex);
            time = path.pathCreator.path.GetTimeByIndexAndNextIndex(followIndex);
            positionOnPath = path.pathCreator.path.GetPointAtTime(time);
            distance = Vector3.Distance(positionOnPath, position.current);
        }
        normal = path.pathCreator.path.GetNormal(time);
        //Debug.DrawLine(positionOnPath, positionOnPath + (normal * 20), Color.green);

        AddForce(height - Vector3.Project(positionOnPath - position.current, normal).magnitude);
    }

    public void SetDirection(Vector3 direction)
    {
        direction.Normalize();
        this.direction = direction;
    }

    private void FixedUpdate()
    {
        position.SetCurrent(this.transform.position);
        if (automatic)
        {
            SetDirection(-CustomGravity.GetUpAxis(position.current));
            Hover(targetDistance, currentLinker);
        }
        if (firing)
        {
            effect.SetVector3("spawn", position.current);
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
        position.UpdateOld();
    }
}