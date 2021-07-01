using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HGSLevelEditor
{
    public class GhostObjectMaker : MonoBehaviour
    {
        public GameObject targetObject;
        public static GhostObjectMaker instance;
        public static GhostObjectMaker GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
            
        }
        public void SpawnObject(GameObject objectSpawn)
        {

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
}

