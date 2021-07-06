using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HGSLevelEditor;
public class DemoDropdown : MonoBehaviour
{

    //public Dropdown gameDropdown; 
    public GameObject trainHead;
    public GameObject turret;
    public GameObject flatRock;

    Dropdown gameDropdown;
    public GameObject targetObject;

    GhostObjectMaker ghost;

    private enum Meshes { 
    
        TrainHead = 1,    
        Turret = 2,
        FlatRock = 3 
       

    }

    //public GameObject targetAccess;
    public void Start()
    {
        ghost = GhostObjectMaker.GetInstance();
        gameDropdown = GetComponent<Dropdown>();


    }
    // Update is called once per frame
    public void Update()
    {
        // Cube 
        if (gameDropdown.value == (int)Meshes.TrainHead)
        {
            gameDropdown.value = 0;
            ghost.SpawnGhost(trainHead);
        }
        else if (gameDropdown.value == (int)Meshes.Turret)
        {
            gameDropdown.value = 0;
            ghost.SpawnGhost(turret);
        }
        else if (gameDropdown.value == (int)Meshes.FlatRock) {
            
            gameDropdown.value = 0;
            ghost.SpawnGhost(flatRock);
        }

    }
}
