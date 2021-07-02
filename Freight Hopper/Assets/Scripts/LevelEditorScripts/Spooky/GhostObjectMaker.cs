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

            GameObject ghost = new GameObject("Ghost " + objectSpawn.name);
            string id = objectSpawn.GetComponent<LevelObjectInfo>().data.objectID;

            ghost.transform.position = objectSpawn.transform.position;
            // Check parent for mesh filter, copy it, save its location and move the new parent to location
            if (objectSpawn.GetComponent<MeshFilter>() != null)
                {
                    GameObject child = new GameObject(objectSpawn.name);

                    child.transform.localPosition = Vector3.zero;
                    child.AddComponent<MeshFilter>().mesh = objectSpawn.transform.gameObject.GetComponent<MeshFilter>().sharedMesh;
                    child.AddComponent<MeshRenderer>().material = objectSpawn.transform.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                }

                // Check children of parent for mesh filters, copy them if applicable, save locations, and move to new locations while keeping it as parent
                CopyMeshes(objectSpawn, ghost);


                ghost.AddComponent<LevelObjectInfo>();

                Debug.Log("Object ID for debug: " + objectSpawn.GetComponent<LevelObjectInfo>().data.objectID);

                ghost.GetComponent<LevelObjectInfo>().data = objectSpawn.GetComponent<LevelObjectInfo>().GetObject();
            
                return ghost;
            
        }

        private void CopyMeshes(GameObject oldParent, GameObject ghost)
        {
            for (int i = 0; i < oldParent.transform.childCount; i++)
            {
                GameObject oldChild = oldParent.transform.GetChild(i).gameObject;

                if (oldChild.GetComponent<MeshFilter>() != null)
                {
                    Vector3 globalLocation = oldChild.transform.position;
                    GameObject child = new GameObject(oldChild.name);

                    child.transform.position = globalLocation;
                    child.transform.parent = ghost.transform;
                    child.AddComponent<MeshFilter>().mesh = oldChild.transform.gameObject.GetComponent<MeshFilter>().sharedMesh;
                    child.AddComponent<MeshRenderer>().material = oldChild.transform.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                }
                CopyMeshes(oldChild, ghost);
            }
        }


        public GameObject ReturnGhost(GameObject spooky) {
            
            return MakeGhost(spooky);
        
        }

        public void SpawnGhost(GameObject spooky) {

            GameObject ghost = MakeGhost(spooky);

            ghost.transform.position = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, targetObject.transform.position.z);

            Instantiate(ghost,
               new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, targetObject.transform.position.z),
               ghost.transform.rotation);

            Destroy(ghost);
        }

        
    }
}

