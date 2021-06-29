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

        private GameObject[] otherUI;
        public Button exit;

        private void Start()
        {
            Button x = exit.GetComponent<Button>();
            x.onClick.AddListener(ExitOnClick);

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
            SceneManager.LoadScene("CustomLevel 0 0", LoadSceneMode.Single);

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
