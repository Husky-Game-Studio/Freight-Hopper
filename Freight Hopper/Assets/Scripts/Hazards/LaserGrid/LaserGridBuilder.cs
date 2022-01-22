using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserGridBuilder : MonoBehaviour
{
    [SerializeField] private GameObject poleObject;
    [SerializeField] private GameObject laserObject;
    [SerializeField, Min(1)] private int layers = 8;
    [SerializeField, Min(4)] private float width = 10f;
    [SerializeField, Min(0.1f)] private float laserThickness = 0.2f;
    [SerializeField] private bool allLayersActive = false;
    [SerializeField] private List<int> activeLayers;
    
    private const float GRID_SCALAR = 2.25f;
    private const float LASER_SHIFTER = 1.86f;

    
    private bool laserGroupOpen = false;
    private int laserGroupCounter = 0;

    public void OnValidate()
    {
        bool validArguments = CheckArguments();
    }

    private bool CheckArguments()
    {
        if (poleObject == null || laserObject == null)
        {
            Debug.Log("poleObject or laserObject is null");
            return false;
        }
        if (layers < 1)
        {
            Debug.Log("layers must be greater than 0");
            return false;
        }
        /*if (activeLayers != null && activeLayers.Count > layers)
        {
            Debug.Log("activeLayers must be <= to " + layers);
            return false;
        }*/
        return true;
    }

    private void SetBoxCollider(int layer)
    {
        Debug.Log("In SetBoxCollider("+ layer + ")");
        BoxCollider laserGroupCollider = this.gameObject.AddComponent<BoxCollider>();

        Vector3 currentPosition = Vector3.zero;
        Vector3 boxPosition = currentPosition;
        boxPosition += transform.up * layer * GRID_SCALAR;
        boxPosition += transform.up * LASER_SHIFTER;

        float cumulativeYValue = 0;
        for (int i = 0; i < laserGroupCounter; i++)
        {
            cumulativeYValue += layer - i;
        }
        cumulativeYValue /= laserGroupCounter;
        laserGroupCollider.center = boxPosition;
    }
    
    private void BuildLaserLayer(int layer, bool activeLayer)
    {
        Vector3 currentPosition = this.gameObject.transform.position;
        Quaternion currentQuaternion = this.gameObject.transform.rotation;
        
        Vector3 polePosition = currentPosition + this.gameObject.transform.right * width / 2;
        polePosition += (transform.up * layer * GRID_SCALAR);
        
        Instantiate(poleObject, polePosition, currentQuaternion, this.gameObject.transform);
        Instantiate(poleObject, polePosition - transform.right * width, Quaternion.LookRotation(-transform.forward, transform.up), this.gameObject.transform);
        
        if (activeLayer)
        {
            Vector3 laserPosition = currentPosition;
            laserPosition += transform.up * layer * GRID_SCALAR;
            laserPosition += transform.up * LASER_SHIFTER;
            GameObject currentLaser = Instantiate(laserObject, laserPosition, currentQuaternion, this.gameObject.transform);
            currentLaser.transform.localScale = new Vector3(width, laserThickness, laserThickness);

            laserGroupOpen = true;
            laserGroupCounter++;
        }
        else
        {
            if (laserGroupOpen)
            {
                SetBoxCollider(layer);
            }
            laserGroupOpen = false;
            laserGroupCounter = 0;
        }
        
        
    }
    
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
                SetBoxCollider(i);
                laserGroupOpen = false;
                laserGroupCounter = 0;
            }
        }
    }
}
