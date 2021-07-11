using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HGSLevelEditor
{
    [System.Serializable]
    public class SaveLevelInput : MonoBehaviour
    {
        [SerializeField]
        private InputField input;
        public GameObject menu;
        public Text levelText; 
        
        string levelName;

        public void SaveLevel()
        {
            levelName = input.text;
            LevelManager.levelNameLoad = levelName;
            SaveLoadLevel.GetInstance().SaveLevelButton(levelName);
            levelText.text = "Current Level: " + levelName;

        }

        public void CloseMenu() {

            menu.SetActive(false);
        }
    }
}
