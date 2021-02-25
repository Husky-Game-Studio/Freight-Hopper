using UnityEngine;

[System.Serializable]
public class HoverEngine : MonoBehaviour
{
    [SerializeField, ReadOnly] private Rigidbody rb;
    public PID controller;
    public float targetDistance;
    public Vector3 direction;
    public LayerMask layerMask;
    public bool automatic;

    private void Awake()
    {
        if (transform.parent.parent.GetComponent<Rigidbody>() != null)
        {
            rb = transform.parent.parent.GetComponent<Rigidbody>();
        }
    }

    private void Reset()
    {
        targetDistance = 3;

        layerMask = LayerMask.GetMask("Default");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position, this.transform.position + direction * targetDistance);
    }

    public void Hover(float height)
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, direction, out hit, height + 0.1f, layerMask))
        {
            float error = height - hit.distance;
            Debug.DrawLine(this.transform.position, this.transform.position + Vector3.up * error, Color.white);
            // We don't want the hover engine to correct itself downwards. Hovering only applys upwards!
            if (error > -0.1f)
            {
                Vector3 force = -direction * this.controller.GetOutput(error, Time.fixedDeltaTime);

                rb.AddForceAtPosition(force, this.transform.position, ForceMode.Force);
            }
        }
    }

    private void FixedUpdate()
    {
        if (automatic)
        {
            direction = Physics.gravity.normalized;
            Hover(targetDistance);
        }
    }
}