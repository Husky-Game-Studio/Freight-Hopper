using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModularLaserGridBehavior : MonoBehaviour
{
    [SerializeField] private GameObject poleObject;
    [SerializeField] private GameObject laserObject;
    [SerializeField] private int layers = 8;
    [SerializeField] private List<int> activeLayers;
    [SerializeField] private float width = 10f;
    [SerializeField] private bool allLayersActive = false;
    [SerializeField] private const int MIN_WIDTH = 4;

    private bool firstLaserInGroup = true;
    private int laserGroupCounter = 0;
    

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

    private void BuildLaserLayer(int layer, bool activeLayer)
    {
        Instantiate(poleObject, new Vector3(width / -2, layer, 0), Quaternion.identity, this.gameObject.transform);
        Instantiate(poleObject, new Vector3(width / 2, layer, 0), Quaternion.identity, this.gameObject.transform);
        if (activeLayer)
        {
            Instantiate(laserObject, new Vector3(0, layer, 0), Quaternion.identity, this.gameObject.transform);
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
                BoxCollider laserGroupCollider = this.gameObject.AddComponent<BoxCollider>();
                float cumulativeYValue = 0;
                for (int i = 0; i < laserGroupCounter; i++)
                {
                    cumulativeYValue += layer - i;
                }
                cumulativeYValue /= laserGroupCounter;
                laserGroupCollider.transform.position = new Vector3(0, cumulativeYValue, 0);
            }
            laserGroupCounter = 0;
        }
    }
    
    private void BuildLaserGrid()
    {
        if (allLayersActive|| activeLayers == null)
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
        }
    }
}
