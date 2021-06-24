using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HGSLevelEditor
{
    public class LevelManager : MonoBehaviour
    {

        //Don't want them to show, but I need to change it 

        public List<LevelObjects> o = new List<LevelObjects>();
        public int index;

        private static LevelManager instance;
        public static LevelManager GetInstance()
        {

            return instance;
        }

        void Awake()
        {
            instance = this;
        }

        //Might not need this 
        void storeLevelObjects()
        {

            if (o[index].inSceneObjects.Count > 0)
            {

                for (int i = 0; i < o[index].inSceneObjects.Count; i++)
                {

                    LevelObjectInfo ob = o[index].inSceneObjects[i].GetComponent<LevelObjectInfo>();

                }
            }
        }
        public void ClearLevel()
        {

            foreach (LevelObjects obj in o)
            {

                ClearLevelActual(obj);
            }
            o.Clear();

        }

        void ClearLevelActual(LevelObjects current)
        {

            foreach (GameObject obj in current.inSceneObjects)
            {

                Destroy(obj);

            }

            current.inSceneObjects.Clear();
            Destroy(current.obj);
        }

        [System.Serializable]
           public class LevelObjects
        {
                                                                                                                                                        
            public GameObject obj;
            public List<GameObject> inSceneObjects = new List<GameObject>();


        }
    }
}
