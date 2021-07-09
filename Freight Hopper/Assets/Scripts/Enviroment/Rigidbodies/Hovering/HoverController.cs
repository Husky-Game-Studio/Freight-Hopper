using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    [SerializeField, ReadOnly] private Rigidbody rb;

    [SerializeField, ReadOnly, Tooltip("Automatically grabs hover engine that are children to this object")] private List<HoverEngine> hoverEnginePivots;
    [SerializeField] private HoverPresets hoverSetting;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float targetDistance;
    [SerializeField] private bool automatic = true;
    [SerializeField] private GameObject hoverEnginePrefab;
    [SerializeField, ReadOnly] private bool enginesFiring;

    public bool EnginesFiring => enginesFiring;

#if UNITY_EDITOR

    public void AddHoverEngine(Vector3 position, string name = "")
    {
        if (string.IsNullOrEmpty(name))
        {
            name = hoverEnginePivots.Count.ToString();
        }
        GameObject go = PrefabUtility.InstantiatePrefab(hoverEnginePrefab) as GameObject;
        go.name = name;
        go.transform.parent = this.transform;
        go.transform.localPosition = position;
        InitializeEngines();
    }

#endif

    private void Awake()
    {
        InitializeEngines();
    }

    private void OnValidate()
    {
        InitializeRigidBody();
        if (this.gameObject != null)
        {
            InitializeEngines();
        }
    }

    private void Reset()
    {
        layerMask = LayerMask.GetMask("Default");
        InitializeRigidBody();

        targetDistance = 3;

        InitializeEngines();
    }

    private void InitializeRigidBody()
    {
        if (this.transform.parent.GetComponent<Rigidbody>() != null)
        {
            rb = this.transform.parent.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogWarning("HoverController: Rigidbody not found in parent", this.gameObject);
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
            hoverEngine.Initialize(rb, layerMask, hoverSetting.CurrentPreset(), targetDistance, automatic);
        }
    }

    //  arrigato
    private void FixedUpdate()
    {
        enginesFiring = false;
        foreach (HoverEngine eng in hoverEnginePivots)
        {
            enginesFiring |= eng.Firing;
        }
    }
}