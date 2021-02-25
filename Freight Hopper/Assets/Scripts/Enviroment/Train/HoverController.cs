using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    [SerializeField, ReadOnly] private Rigidbody rb;

    [SerializeField, ReadOnly, Tooltip("Automatically grabs hover engine that are children to this object")] private List<HoverEngine> hoverEnginePivots;
    [SerializeField] private HoverPresets hoverSetting;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float targetDistance;
    [SerializeField] private bool automatic = true;

    public void AddHoverEngine(Vector3 position, string name = "")
    {
        if (name.Equals(""))
        {
            name = hoverEnginePivots.Count.ToString();
        }
        GameObject go = new GameObject(name);
        go.transform.parent = this.transform;
        go.transform.localPosition = position;
        go.AddComponent<HoverEngine>();
        InitializeEngines();
    }

    private void OnValidate()
    {
        if (this.gameObject != null)
        {
            InitializeEngines();
        }
    }

    private void Reset()
    {
        layerMask = LayerMask.GetMask("Default");
        if (transform.parent.GetComponent<Rigidbody>() != null)
        {
            rb = transform.parent.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogWarning("HoverController: Rigidbody not found in parent");
        }

        targetDistance = 3;

        InitializeEngines();
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
}