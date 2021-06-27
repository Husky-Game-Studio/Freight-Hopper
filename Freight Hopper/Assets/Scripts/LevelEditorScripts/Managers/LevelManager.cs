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
        public static string levelNameLoad;

        private static LevelManager instance;
        public static LevelManager GetInstance()
        {

            return instance;
        }

        void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            instance = this;

            if (levelNameLoad != null) {

                SaveLoadLevel.GetInstance().LoadButton(levelNameLoad);

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
