using System.Collections.Generic;
using UnityEngine;

public class HoverEngines : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxDistance;
    [SerializeField] private float targetDistance;

    [Tooltip("Normal of engine direction")]
    [SerializeField] private Vector3 direction;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private string centerOfMassGameObjectName;

    [SerializeField] private List<HoverEngine> hoverEnginePivots;
    [SerializeField] private float p, i, d;

    private void Awake()
    {
        rb.centerOfMass = centerOfMass.localPosition;
        InitializeEngines();
    }

    private void InitializeEngines()
    {
        foreach (HoverEngine hoverEngine in hoverEnginePivots)
        {
            hoverEngine.Initailize(p, i, d);
        }
    }

    private void OnValidate()
    {
        InitializeEngines();
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
        direction = Vector3.down;
        maxDistance = 5;
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
        targetDistance = 2;
        p = 300000;
        i = 600;
        d = 5000;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (HoverEngine pivot in hoverEnginePivots)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pivot.transform.position, pivot.transform.position + direction * maxDistance);
        }
    }

    private void FixedUpdate()
    {
        foreach (HoverEngine pivot in hoverEnginePivots)
        {
            RaycastHit hit;
            if (Physics.Raycast(pivot.transform.position, direction, out hit, maxDistance, layerMask))
            {
                //Vector3 force = -direction * (forceMultiplier / hit.distance);
                float error = targetDistance - hit.distance;
                if (error > 0)
                {
                    Vector3 force = -direction * pivot.controller.GetOutput(error, Time.deltaTime);
                    rb.AddForceAtPosition(force, pivot.transform.position, ForceMode.Force);
                }

                //Debug.Log(hit.distance);
                //Debug.Log(pivot.gameObject.name + " - force: " + force);
            }
        }
    }
}