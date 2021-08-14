using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HGSLevelEditor {

    public class ObjectManager : MonoBehaviour
    {
        //List holds info. for all level objects available in Level Editor 
        public List<LevelObject> LevelObjects = new List<LevelObject>();

        private static ObjectManager instance = null;

        //Allows us to make buttons for the object UI 
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

            //Checks if object is already in the objectManager List
            bool found = false;

            foreach (LevelEditorObjects eObj in eObj.objects)
            {
                //Search through ObjectManager and see if object ID exists 
                for (int i = 0; i < LevelObjects.Count; i++)
                {
                    if (eObj.objectID == LevelObjects[i].objID)
                    {
                        found = true;
                    }
                }
                //If not, add it 
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

        public List<LevelObject> GetObjectList() {

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

