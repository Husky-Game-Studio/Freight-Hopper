using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HGSLevelEditor {

    public class ObjectManager : MonoBehaviour
    {
        //List holds info. for all level objects available in Level Editor 
        public List<LevelObject> LevelObjects = new List<LevelObject>();

        private static ObjectManager instance = null;

        public ObjectCollection eObj;


        void Awake()
        {
            instance = this;
            AudioListener.volume = 0;
            ScanForNew();
        }

        public static ObjectManager GetInstance()
        {

            return instance;
        }

        public void ScanForNew() {

            Debug.Log("We in it maybe");

            Debug.Log("Numba:" + eObj.objects.Length);
            bool found = false;

            foreach (LevelEditorObjects eObj in eObj.objects)
            {
                Debug.Log("We in it");
                //Search through ObjectManager and see if object ID exists -- use a boolean? 
                for (int i = 0; i < LevelObjects.Count; i++)
                {
                    if (eObj.objectID == LevelObjects[i].objID)
                    {
                        found = true;
                    }
                }

                if (found == false)
                {
                    AddToList(eObj.objectID, eObj.objectPrefab);
                }
            }
        }

        //Will return the obj's prefab within the list, based off the objectID given.
        public LevelObject GetObject(string objectID) {

            LevelObject retrieve = new LevelObject();

            for (int i = 0; i < LevelObjects.Count; i++) {

                if (objectID.Equals(LevelObjects[i].objID)) {

                    retrieve = LevelObjects[i];
                    break;
                }
            }

            if (retrieve == null) {

                Debug.Log("Retrieve is NULL");
            }

            return retrieve;
        }

        public List<LevelObject> getObjectList() {

            return LevelObjects; 
        }

        public void AddToList(string newObjID, GameObject newObjPrefab) {

            LevelObjects.Add(new LevelObject { objID = newObjID, objPrefab = newObjPrefab});
        }

        //Stores object ID + prefab information
        [System.Serializable]
        public class LevelObject
        {
            public string objID;
            public GameObject objPrefab;
        }
    }
}

