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
        public GameObject MakeGhost(GameObject objectSpawn)
        {

            GameObject spook = new GameObject("spook");
            Debug.Log("spook made!");

            if (objectSpawn.transform.childCount > 0)
            {

                spook.AddComponent<MeshFilter>().mesh = objectSpawn.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().sharedMesh;
                spook.AddComponent<MeshRenderer>().material = objectSpawn.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;

            }
            else {

                spook.AddComponent<MeshFilter>().mesh = objectSpawn.transform.gameObject.GetComponent<MeshFilter>().sharedMesh;
                spook.AddComponent<MeshRenderer>().material = objectSpawn.transform.gameObject.GetComponent<MeshRenderer>().sharedMaterial;

            }
            
            spook.AddComponent<LevelObjectInfo>();

            Debug.Log("Object ID for debug: " + objectSpawn.GetComponent<LevelObjectInfo>().data.objectID);

            spook.GetComponent<LevelObjectInfo>().data = objectSpawn.GetComponent<LevelObjectInfo>().GetObject();
            //spook.GetComponent<LevelObjectInfo>().data.objectID = objectSpawn.GetComponent<LevelObjectInfo>().GetID();
           
            return spook;
            
        }

        public GameObject ReturnGhost(GameObject spooky) {
            
            return MakeGhost(spooky);
        
        }

        public void SpawnGhost(GameObject spooky) {

            GameObject spook = MakeGhost(spooky);

            spook.transform.position = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, targetObject.transform.position.z);

            Instantiate(spook,
               new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, targetObject.transform.position.z),
               spook.transform.rotation);

            Destroy(spook);
        }
    }
}

