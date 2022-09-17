using System;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class LaserGridBuilder : MonoBehaviour
{
    [SerializeField] private GameObject poleObject;
    [SerializeField] private GameObject laserObject;
    [SerializeField, Min(1)] private int layers = 8;
    [SerializeField, Min(4)] private float width = 10f;
    [SerializeField, Min(0.1f)] private float laserThickness = 0.2f;
    [SerializeField] private bool allLayersActive;
    [SerializeField] private List<int> activeLayers;
    private List<Vector3> activeLayersPositions;
    
    private const float GRID_SCALAR = 2.25f;
    private const float LASER_SHIFTER = 1.86f;

    
    private bool laserGroupOpen;
    private int laserGroupCounter;

    // Validate the arguments are valid
    public void OnValidate()
    {
        laserGroupOpen = false;
        laserGroupCounter = 0;
        activeLayersPositions = new List<Vector3>();
        CheckArguments();
    }

    private void CheckArguments()
    {
        if (poleObject == null || laserObject == null)
        {
            Debug.Log("poleObject or laserObject is null");
        }
        if (layers < 1)
        {
            Debug.Log("layers must be greater than 0");
        }
    }

    // Set all the box colliders for this laser grid.
    // Only creates box colliders for groups of lasers. So if the entire grid have active lasers, there will only
    // be one box collider. If there are gaps without active lasers, each group will have their own box collider
    private void SetBoxCollider()
    {
        BoxCollider laserGroupCollider = this.gameObject.AddComponent<BoxCollider>();

        Vector3 boxLocation = Vector3.zero;
        int activeLayerCounter = 0;
        foreach (Vector3 activeLayer in activeLayersPositions)
        {
            boxLocation += activeLayer;
            activeLayerCounter++;
        }

        float x = boxLocation.x;
        float y = boxLocation.y;
        float z = boxLocation.z;
        x /= activeLayerCounter;
        y /= activeLayerCounter;
        z /= activeLayerCounter;
        boxLocation = new Vector3(x, y, z);// - transform.position;

        laserGroupCollider.center = boxLocation;
        
        activeLayersPositions.Clear();
        
        Vector3 currentSize = new Vector3(width, (laserGroupCounter * GRID_SCALAR) - GRID_SCALAR, laserThickness * 1.5f);
        laserGroupCollider.size = currentSize;
    }
    
    // Creates one layer for the laser grid
    private void BuildLaserLayer(int layer, bool activeLayer)
    {
        var o = this.gameObject;
        Vector3 currentPosition = o.transform.position;
        Quaternion currentQuaternion = o.transform.rotation;
        
        Vector3 polePosition = currentPosition + o.transform.right * width / 2;
        polePosition += (transform.up * (layer * GRID_SCALAR));
        
        Instantiate(poleObject, polePosition, currentQuaternion, this.gameObject.transform);
        var transform1 = transform;
        Instantiate(poleObject, polePosition - transform1.right * width, Quaternion.LookRotation(-transform1.forward, transform1.up), this.gameObject.transform);
        
        if (activeLayer)
        {
            Vector3 laserPosition = currentPosition;
            var up = transform.up;
            laserPosition += up * (layer * GRID_SCALAR);
            laserPosition += up * LASER_SHIFTER;
            
            GameObject currentLaser = Instantiate(laserObject, laserPosition, currentQuaternion, this.gameObject.transform);
            activeLayersPositions.Add(currentLaser.transform.localPosition);
            currentLaser.transform.localScale = new Vector3(width, laserThickness, laserThickness);

            laserGroupOpen = true;
            laserGroupCounter++;
        }
        else
        {
            if (laserGroupOpen)
            {
                SetBoxCollider();
            }
            laserGroupOpen = false;
            laserGroupCounter = 0;
        }

        if (laserGroupOpen && layer == layers -1)
        {
            SetBoxCollider();
            laserGroupOpen = false;
            laserGroupCounter = 0;
        }
    }
    
    // Builds the whole laser grid
    public void BuildLaserGrid()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in this.gameObject.transform)
        {
            children.Add(child.gameObject);
        }
        foreach (GameObject child in children)
        {
            DestroyImmediate(child);
        }

        foreach (BoxCollider boxCollider in gameObject.GetComponents<BoxCollider>())
        {
            DestroyImmediate(boxCollider);
        }
        
        if (allLayersActive || activeLayers == null)
        {
            activeLayers = new List<int>();
            for (int i = 0; i < layers; i++)
            {
                activeLayers.Add(i);
            }
        }

        for (int i = 0; i < layers; i++)
        {
            BuildLaserLayer(i, activeLayers.Contains(i));
            if (i - 1 == layers && laserGroupOpen)
            {
                Debug.Log("Last Layer");
                SetBoxCollider();
                laserGroupOpen = false;
                laserGroupCounter = 0;
            }
        }
    }
}
