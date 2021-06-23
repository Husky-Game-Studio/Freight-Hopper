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

        public LevelObject getObject(string objectID) {

            LevelObject retrieve = null;

            for (int i = 0; i < LevelObjects.Count; i++) {

                if (objectID.Equals(LevelObjects[i].objID)) {

                    retrieve = LevelObjects[i];
                    break;
                }
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

