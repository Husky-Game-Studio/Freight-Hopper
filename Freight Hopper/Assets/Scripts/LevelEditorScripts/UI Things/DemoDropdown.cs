using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoDropdown : MonoBehaviour
{

    //public Dropdown gameDropdown; 
    public GameObject trainHead;
    Dropdown gameDropdown;
    public GameObject targetObject;

    //public GameObject targetAccess;
    public void Start()
    {
        gameDropdown = GetComponent<Dropdown>();

    }
    // Update is called once per frame
    public void Update()
    {
        // Cube 
        if (gameDropdown.value == 1)
        {
            Debug.Log("Train Head Spawned");
            Instantiate(trainHead,
            new Vector3(targetObject.transform.position.x, trainHead.transform.position.y, targetObject.transform.position.z),
            trainHead.transform.rotation);

            //Reverts back to 0 to prevent endless spawning 
            gameDropdown.value = 0;

        }


    }
}
