using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HGSLevelEditor
{
    public class GridLoadLevelUI : MonoBehaviour
    {
        public Transform LoadLevelButtons;
        public GameObject loadLevelButtonPrefab;

        SaveLoadLevel level;
        string movingLevel = "Hi";
        // Start is called before the first frame update
        private void Start()
        {
            CloseLoadLevelButtons();

            level = SaveLoadLevel.GetInstance();

            CreateUIButtonsForLevels();
        }

        void CreateUIButtonsForLevels()
        {

            foreach (string name in level.allLevels)
            {

                GameObject buttons = Instantiate(loadLevelButtonPrefab) as GameObject;
                buttons.transform.SetParent(LoadLevelButtons);

                LoadLevelUI levelButton = buttons.GetComponent<LoadLevelUI>();
                levelButton.levelName = name;
                Debug.Log(name + "Checking");
                levelButton.SetText(name);
                movingLevel = name;

            }
        }

        public void OpenCloseLevelButtons()
        {

            if (LoadLevelButtons.gameObject.activeInHierarchy)
            {

                CloseLoadLevelButtons();

            }
            else
            {

                OpenLoadLevelButtons();

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
