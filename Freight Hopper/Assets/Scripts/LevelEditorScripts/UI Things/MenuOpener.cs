using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


// I need to rename this script -- to something like ButtonManager -- if it isn't named that by the time you see this, ssshhh, no you didn't <3 

namespace HGSLevelEditor
{
    public class MenuOpener : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField] private GameObject menu;

        public GameObject[] otherUI;
       
        SaveLoadLevel level;
        PlayButton play;

        string movingLevel = "Hi"; 
        public Button exit;

        private void Start()
        {
            Button x = exit.GetComponent<Button>();
            x.onClick.AddListener(ExitOnClick);

            level = SaveLoadLevel.GetInstance();
            play = PlayButton.GetInstance();

        }

        public void OpenMenu()
        {

            if (menu != null)
            {

                menu.SetActive(true);

            }
        }

        public void PlayLevel()
        {
            //need to pass level name ...this probably is not the place for it ... ]
            //play.LoadAtPlay(movingLevel); -- not workin rn -- oops

            SceneManager.LoadScene("CustomLevel 0 0", LoadSceneMode.Single);
            SceneManager.LoadScene("DefaultScene", LoadSceneMode.Additive);

        }
        public void ExitOnClick()
        {

            menu.SetActive(false);

        }

       
        public static MenuOpener instance;
        public static MenuOpener GetInstance() {

            return instance;
        }

        void Awake()
        {
            instance = this;     
        }

        public void NewLevel() {

            LevelManager.GetInstance().ClearLevel();
        
        }

    }

}
