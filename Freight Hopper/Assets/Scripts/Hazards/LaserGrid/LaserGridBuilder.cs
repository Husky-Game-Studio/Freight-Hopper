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
    [SerializeField] private List<int> activeLayers;
    [SerializeField] private float width = 10f;
    [SerializeField] private bool allLayersActive = false;
    [SerializeField] private const int MIN_WIDTH = 4;

    private bool firstLaserInGroup = true;
    private int laserGroupCounter = 0;

    public void OnValidate()
    {
        bool validArguments = CheckArguments();
        if (!validArguments)
        {
            return;
        }
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
        if (width < MIN_WIDTH)
        {
            Debug.Log("width must be >= to " + MIN_WIDTH);
            return false;
        }
        if (activeLayers != null && activeLayers.Count > layers)
        {
            Debug.Log("activeLayers must be <= to " + layers);
            return false;
        }
        return true;
    }

    private void SetBoxCollider(int layer, Vector3 currentPostition)
    {
        /*BoxCollider laserGroupCollider = this.gameObject.AddComponent<BoxCollider>();
        Debug.Log("box collider exists: " + laserGroupCollider);
        float cumulativeYValue = 0;
        for (int i = 0; i < laserGroupCounter; i++)
        {
            cumulativeYValue += layer - i;
        }
        cumulativeYValue /= laserGroupCounter;
        laserGroupCollider.center = new Vector3(currentPostition.x, currentPostition.y + cumulativeYValue, currentPostition.z);*/
    }
    
    private void BuildLaserLayer(int layer, bool activeLayer)
    {
        Vector3 currentPostition = this.gameObject.transform.position;
        Quaternion currentQuaternion = this.gameObject.transform.rotation;
        
        Vector3 polePosition = currentPostition + this.gameObject.transform.right * width / 2;
        polePosition += transform.up * layer;
        
        Instantiate(poleObject, polePosition, currentQuaternion, this.gameObject.transform);
        Instantiate(poleObject, polePosition - transform.right * width, Quaternion.LookRotation(-transform.forward, transform.up), this.gameObject.transform);
        
        if (activeLayer)
        {
            Vector3 laserPosition = currentPostition;
            laserPosition += transform.up * layer;
            Instantiate(laserObject, laserPosition, currentQuaternion, this.gameObject.transform);
            if (firstLaserInGroup)
            {
                firstLaserInGroup = false;
                
            }

            laserGroupCounter++;
        }
        else
        {
            firstLaserInGroup = true;
            if (laserGroupCounter > 0)
            {
                SetBoxCollider(layer, currentPostition);
                /*BoxCollider laserGroupCollider = this.gameObject.AddComponent<BoxCollider>();
                Debug.Log("box collider exists: " + laserGroupCollider);
                float cumulativeYValue = 0;
                for (int i = 0; i < laserGroupCounter; i++)
                {
                    cumulativeYValue += layer - i;
                }
                cumulativeYValue /= laserGroupCounter;
                laserGroupCollider.center = new Vector3(currentPostition.x, currentPostition.y + cumulativeYValue, currentPostition.z);*/
            }
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
            /*if (layers == i - 1 && laserGroupCounter > 0 && !firstLaserInGroup)
            {
                firstLaserInGroup = true;
                SetBoxCollider(i, this.gameObject.transform.position);
                laserGroupCounter = 0;
            }*/
        }
    }
}
