using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using HGSLevelEditor;
using UnityEngine.SceneManagement;
using System.IO;

public class LoadLevelUI : MonoBehaviour
{
    [SerializeField] private Text txt;
    public string levelName;

    [SerializeField] private GameObject validatingMenu;

    
    public void DeleteLevel() {

        File.Delete(Application.streamingAssetsPath + "/Levels" + "/" + LevelManager.levelNameLoad);
        GridLoadLevelUI.GetInstance().ReloadLevels();
        SetMenu();
    }

    public void LoadLevel() {

        //Need this for when I'm going back to the first iteration of the level editor 
        // SceneManager.LoadScene("LevelEditorBeginning");
        SceneManager.LoadScene("LevelEditorIteration3");
        SaveLoadLevel.GetInstance().LoadButton(LevelManager.levelNameLoad, true);
        GridLoadLevelUI.GetInstance().CloseLoadLevelButtons();
        //LevelManager.levelNameLoad = levelName;
    }

    public void SetText(string newText) {
        txt.text = newText; 
    }

    public void SetLevelName() {

        LevelManager.levelNameLoad = levelName;
        Debug.Log ("levelName selected: " + LevelManager.levelNameLoad);
    }

    public void SetMenu() {

        if (validatingMenu.activeInHierarchy == false) {

            validatingMenu.SetActive(true);
        }
        else {

            validatingMenu.SetActive(false);
        }
    }
}
