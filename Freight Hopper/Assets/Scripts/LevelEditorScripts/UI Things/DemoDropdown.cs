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

    private enum Meshes { 
    
        TrainHead = 1,    
        Turret = 2,
        FlatRock = 3 

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
            SpawnObject(trainHead);
        }
        else if (gameDropdown.value == (int)Meshes.Turret)
        {
            SpawnObject(turret);
        }
        else if (gameDropdown.value == (int)Meshes.FlatRock) { 
        
            SpawnObject(flatRock);
        }

    }

    //Most likely need to move in its own class -- Ghost Objects, o O o o oOo o o oOoo !! 
    private void SpawnObject(GameObject objectSpawn) {

        gameDropdown.value = 0;

        GameObject spook = new GameObject("spook");
        Debug.Log("spook made!");

        spook.AddComponent<MeshFilter>().mesh = objectSpawn.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().sharedMesh;
        spook.AddComponent<MeshRenderer>().material = objectSpawn.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;
        spook.AddComponent<LevelObjectInfo>();

        Debug.Log("Object ID for debug: " + objectSpawn.GetComponent<LevelObjectInfo>().data.objectID);

        spook.transform.position = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, targetObject.transform.position.z);

        spook.GetComponent<LevelObjectInfo>().data = objectSpawn.GetComponent<LevelObjectInfo>().GetObject();
        //spook.GetComponent<LevelObjectInfo>().data.objectID = objectSpawn.GetComponent<LevelObjectInfo>().GetID();

        Instantiate(spook,
            new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, targetObject.transform.position.z),
            spook.transform.rotation);

        Destroy(spook);

    }
}
