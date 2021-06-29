using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HGSLevelEditor
{
    //Attaches to the 'LevelEditorBeginning' scene -- acts as the scene's manager
    public class LevelManager : MonoBehaviour
    {

        public List<LevelObjects> o = new List<LevelObjects>();
        public int index;

        //This variable is what allows the level to be played within the 'Play' scene 
        //and also allows the level to reload when the user exits the 'Play' scene 

        public static string levelNameLoad;

        private static LevelManager instance;
        public static LevelManager GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            //Needed to do this because I believe the cursor.visible was set to false when the user hits 'Play',
            //So the cursor would be gone when the user returned to the editor.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            instance = this;

            //Will reload the level if the user is coming back from the 'Play' scene 
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