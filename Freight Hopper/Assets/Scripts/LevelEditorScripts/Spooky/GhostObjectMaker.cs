using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

            ghost.transform.position = objectSpawn.transform.position;
            // Check parent for mesh filter, copy it, save its location and move the new parent to location
            if (objectSpawn.GetComponent<MeshFilter>() != null)
                {
                    Debug.Log("This is being accessed eek!");
                //GameObject child = new GameObject(objectSpawn.name);

                    ghost.transform.localPosition = objectSpawn.transform.position;
                    ghost.AddComponent<MeshFilter>().mesh = objectSpawn.transform.gameObject.GetComponent<MeshFilter>().sharedMesh;
                    ghost.AddComponent<MeshRenderer>().material = objectSpawn.transform.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                    ghost.AddComponent<BoxCollider>();
                    ghost.AddComponent<Outline>();
                    ghost.GetComponent<Outline>().OutlineWidth = 0.0f;
                    

                }

                // Check children of parent for mesh filters, copy them if applicable, save locations, and move to new locations while keeping it as parent
                CopyMeshes(objectSpawn, ghost);


            ghost.AddComponent<LevelObjectInfo>();

            ghost.GetComponent<LevelObjectInfo>().data = objectSpawn.GetComponent<LevelObjectInfo>().GetObject();
            ghost.GetComponent<LevelObjectInfo>().SetID(objectSpawn.GetComponent<LevelObjectInfo>().objID);


            Debug.Log("Object ID for debug: " + ghost.GetComponent<LevelObjectInfo>().objID);

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
                    child.AddComponent<BoxCollider>();
                    child.AddComponent<Outline>();
                    child.GetComponent<Outline>().OutlineWidth = 0.0f;

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

