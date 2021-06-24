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

        string levelName;
        public void SaveLevel()
        {
            levelName = input.text;
            SaveLoadLevel.GetInstance().SaveLevelButton(levelName);

        }

        public void closeMenu() {

            menu.SetActive(false);
        }
    }
}
