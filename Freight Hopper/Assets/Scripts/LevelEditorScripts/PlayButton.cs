using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace HGSLevelEditor
{
    public class PlayButton : MonoBehaviour
    {
        public static PlayButton instance;
        public static PlayButton GetInstance()
        {

            return instance;
        }

        void Awake()
        {
            instance = this;
        }

        public void LoadAtPlay(string levelName) {

            SceneManager.LoadScene("CustomLevel 0 0", LoadSceneMode.Single);
            //Load the game objects 

        }
    }
}
