using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HGSLevelEditor {

    public class ObjectManager : MonoBehaviour
    {
        //List holds info. for all level objects available in Level Editor 
        public List<LevelObject> LevelObjects = new List<LevelObject>();

        private static ObjectManager instance = null;

        void Awake()
        {
            instance = this;
        }

        public static ObjectManager GetInstance()
        {

            return instance;
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

        //Stores object ID + prefab information
        [System.Serializable]
        public class LevelObject
        {
            public string objID;
            public GameObject objPrefab;
        }
    }
}

