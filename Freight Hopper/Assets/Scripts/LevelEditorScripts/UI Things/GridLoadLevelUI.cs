using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script helps create the level buttons under 'Load' at runtime. 

namespace HGSLevelEditor
{
    public class GridLoadLevelUI : MonoBehaviour
    {
        //Transform = Load Grid
        public Transform LoadLevelButtons;
        public GameObject loadLevelButtonPrefab;

        [SerializeField] private GameObject menu;

        //Helps access all level names within the 'Streaming Assets' folder -- in order to create a button for each of the levels.  
        SaveLoadLevel level;
       
        // Start is called before the first frame update
        private void Start()
        {
            CloseLoadLevelButtons();

            level = SaveLoadLevel.GetInstance();

            CreateUIButtonsForLevels();
        }

        //Creates a button for each level
        void CreateUIButtonsForLevels()
        {
            //Prevents function from making a button for the .meta file
            //Searching for .meta code referenced from: https://forum.unity.com/threads/searching-a-string-for-a-string.25878/ 
            foreach (string name in level.allLevels)
            {
                int found;
                int check = 0;
                for (int i = 0; i < name.Length; i++) {

                    found = name.IndexOf("meta", i);
                    if (found > 0)
                    {
                        check++;
                        i = found;
                    }
                }
                if (name != "temp" && check == 0)
                {
                    GameObject buttons = Instantiate(loadLevelButtonPrefab) as GameObject;
                    buttons.transform.SetParent(LoadLevelButtons);

                    LoadLevelUI levelButton = buttons.GetComponent<LoadLevelUI>();
                    levelButton.levelName = name;
                    Debug.Log(name + "Checking");
                    levelButton.SetText(name);

                }
                else {
                    Debug.Log("welp");
                }
            }
        }

        public void OpenCloseLevelButtons()
        {

            if (LoadLevelButtons.gameObject.activeInHierarchy)
            {

                CloseLoadLevelButtons();
                menu.SetActive(false);

            }
            else
            {
                OpenLoadLevelButtons();
                menu.SetActive(true);
            }

        }

        public void CloseLoadLevelButtons()
        {

            LoadLevelButtons.gameObject.SetActive(false);

        }

        public void OpenLoadLevelButtons()
        {

            LoadLevelButtons.gameObject.SetActive(true);

        }

        //Allows the new saved button to be added on the grid at runtime 
        public void ReloadLevels()
        {
            Button[] prev = LoadLevelButtons.GetComponentsInChildren<Button>();
            foreach (Button p in prev)
            {

                Destroy(p.gameObject);
            }

            level.allLevels.Clear();

            level.LoadAllLevels();
            CreateUIButtonsForLevels();
        }

        public static GridLoadLevelUI instance;
        public static GridLoadLevelUI GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
        }
    }
}
