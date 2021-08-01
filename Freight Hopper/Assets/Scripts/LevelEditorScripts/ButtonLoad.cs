using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HGSLevelEditor;
using UnityEngine.UI;

public class ButtonLoad : MonoBehaviour
{
    public GameObject spawnObject;
    GhostObjectMaker ghost;
    public Sprite objectPhoto;
    public string objectID; 
   
    void Start()
    {
        ghost = GhostObjectMaker.GetInstance();
        GetComponent<Button>().GetComponent<Image>().sprite = objectPhoto; 
    }

    public void SpawnObject() {

        if (spawnObject.GetComponent<LevelObjectInfo>() == null) {

            spawnObject.AddComponent<LevelObjectInfo>();
            spawnObject.GetComponent<LevelObjectInfo>().SetID(objectID);
        }
      
        ghost.SpawnGhost(spawnObject);
    }
}
