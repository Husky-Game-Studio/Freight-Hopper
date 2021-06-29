using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoDropdown : MonoBehaviour
{

    //public Dropdown gameDropdown; 
    public GameObject trainHead;
    public GameObject turret; 
    Dropdown gameDropdown;
    public GameObject targetObject;

    private enum Meshes { 
    
        TrainHead = 1,    
        Turret = 2
    }

    //public GameObject targetAccess;
    public void Start()
    {
        gameDropdown = GetComponent<Dropdown>();

    }
    // Update is called once per frame
    public void Update()
    {
        // Cube 
        if (gameDropdown.value == (int)Meshes.TrainHead)
        {
            Debug.Log("Train Head Spawned");
            Instantiate(trainHead,
            new Vector3(targetObject.transform.position.x, trainHead.transform.position.y, targetObject.transform.position.z),
            trainHead.transform.rotation);

            //Reverts back to 0 to prevent endless spawning 
            gameDropdown.value = 0;

        }
        if (gameDropdown.value == (int)Meshes.Turret) {

            Debug.Log("Turret Spawned");
            Instantiate(turret,
            new Vector3(targetObject.transform.position.x, turret.transform.position.y, turret.transform.position.z),
            turret.transform.rotation);

            gameDropdown.value = 0;
        }


    }
}
