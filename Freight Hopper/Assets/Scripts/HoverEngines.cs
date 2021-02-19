using System.Collections.Generic;
using UnityEngine;

public class HoverEngines : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private string centerOfMassGameObjectName;
    [SerializeField] private List<HoverEngine> hoverEnginePivots;

    [Header("Adjustable")]
    [SerializeField, Tooltip("General speed of hovering movement")] private float p;

    [SerializeField, Tooltip("Kind of like increasing rigidity of hovering")] private float i;
    [SerializeField, Tooltip("Kind of like dampening hovering")] private float d;
    [SerializeField, Tooltip("Mask for deciding what the hover engines react to")] private LayerMask layerMask;
    [SerializeField, Tooltip("Target distance above the ground for each engine")] private float targetDistance;
    [SerializeField, Tooltip("Normal of engine direction")] private Vector3 direction;

    private void Awake()
    {
        rb.centerOfMass = centerOfMass.localPosition;
        InitializeEngines();
    }

    private void InitializeEngines()
    {
        foreach (HoverEngine hoverEngine in hoverEnginePivots)
        {
            hoverEngine.controller.Initialize(rb.mass * p, rb.mass * i, rb.mass * d);
        }
    }

    private void OnValidate()
    {
        if (this.gameObject != null)
        {
            InitializeEngines();
        }
    }

    // The default settings for hover engines are initialized.
    // This is assuming
    // A) The children to this gameobject are all hover engine pivot points
    // B) There is a COM or center of mass game object that is the child of the rigidbody
    private void Reset()
    {
        hoverEnginePivots = new List<HoverEngine>();
        foreach (Transform child in this.transform)
        {
            hoverEnginePivots.Add(child.gameObject.GetComponent<HoverEngine>());
        }

        layerMask = LayerMask.GetMask("Default");
        if (transform.parent.GetComponent<Rigidbody>() != null)
        {
            rb = transform.parent.GetComponent<Rigidbody>();
        }

        centerOfMassGameObjectName = "COM";
        foreach (Transform child in rb.transform)
        {
            if (child.name.Equals(centerOfMassGameObjectName))
            {
                centerOfMass = child;
            }
        }

        direction = Vector3.down;
        targetDistance = 3;
        p = 3.5f;
        i = 0.2f;
        d = 1.5f;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (HoverEngine pivot in hoverEnginePivots)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pivot.transform.position, pivot.transform.position + direction * targetDistance);
        }
    }

    private void FixedUpdate()
    {
        Hover();
    }

    private void Hover()
    {
        foreach (HoverEngine pivot in hoverEnginePivots)
        {
            RaycastHit hit;
            if (Physics.Raycast(pivot.transform.position, direction, out hit, targetDistance + 0.1f, layerMask))
            {
                float error = targetDistance - hit.distance;
                Debug.DrawLine(pivot.transform.position, pivot.transform.position + Vector3.up * error, Color.white);
                // We don't want the hover engine to correct itself downwards. Hovering only applys upwards!
                if (error > -0.1f)
                {
                    Vector3 force = -direction * pivot.controller.GetOutput(error, Time.fixedDeltaTime);
                    pivot.force = force.magnitude;
                    rb.AddForceAtPosition(force, pivot.transform.position, ForceMode.Force);
                }
            }
        }
    }
}