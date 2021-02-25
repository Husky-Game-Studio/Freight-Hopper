using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    [SerializeField, ReadOnly] public Rigidbody rb;
    [SerializeField, ReadOnly] private Transform centerOfMass;
    [SerializeField, ReadOnly] private string centerOfMassGameObjectName = "COM";
    [SerializeField] private List<HoverEngine> hoverEnginePivots;

    [SerializeField] private PID.Data PIDData;
    [SerializeField, Tooltip("Mask for deciding what the hover engines react to")] public LayerMask layerMask;
    [SerializeField, Tooltip("Target distance above the ground for each engine")] public float targetDistance;

    [SerializeField] private bool customCenterOfMass = false;
    [SerializeField] private bool automatic = true;

    private void Awake()
    {
        if (customCenterOfMass)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }
    }

    private void InitializeEngines()
    {
        hoverEnginePivots = new List<HoverEngine>();
        foreach (Transform child in this.transform)
        {
            hoverEnginePivots.Add(child.gameObject.GetComponent<HoverEngine>());
        }
        foreach (HoverEngine hoverEngine in hoverEnginePivots)
        {
            if (hoverEngine == null)
            {
                Debug.LogWarning("Hover engine not found");
                continue;
            }

            hoverEngine.controller.Initialize(PIDData * rb.mass);
            hoverEngine.layerMask = layerMask;
            hoverEngine.targetDistance = targetDistance;
            hoverEngine.automatic = automatic;
        }

        foreach (Transform child in rb.transform)
        {
            if (child.name.Equals(centerOfMassGameObjectName))
            {
                centerOfMass = child;
            }
        }
    }

    private void OnValidate()
    {
        if (this.gameObject != null)
        {
            InitializeEngines();
        }
        if (customCenterOfMass)
        {
            Vector3 location = Vector3.zero;
            foreach (HoverEngine engine in hoverEnginePivots)
            {
                location += engine.transform.position;
            }
            location /= hoverEnginePivots.Count;

            if (centerOfMass == null)
            {
                centerOfMass = Instantiate(new GameObject(centerOfMassGameObjectName), location, Quaternion.identity, this.transform.parent).transform;
                print("HoverController: Added Center of Mass Object in the middle of hover engines");
            }
            else
            {
                centerOfMass.position = location;
            }
            rb.centerOfMass = centerOfMass.localPosition;
        }
    }

    private void Reset()
    {
        layerMask = LayerMask.GetMask("Default");
        if (transform.parent.GetComponent<Rigidbody>() != null)
        {
            rb = transform.parent.GetComponent<Rigidbody>();
        }

        customCenterOfMass = false;

        PIDData.Kp = 3.5f;
        PIDData.Ki = 0.2f;
        PIDData.Kd = 1.5f;

        targetDistance = 5;

        InitializeEngines();
    }
}