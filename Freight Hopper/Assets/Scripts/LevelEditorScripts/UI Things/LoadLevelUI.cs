using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using HGSLevelEditor;
using UnityEngine.SceneManagement;

public class LoadLevelUI : MonoBehaviour
{
    [SerializeField]
    private Text txt;
    public string levelName;

    public void LoadLevel() {

        //Need this for when I'm going back to the first iteration of the level editor 
        // SceneManager.LoadScene("LevelEditorBeginning");
        SceneManager.LoadScene("LevelEditorIteration2");
        SaveLoadLevel.GetInstance().LoadButton(levelName, true);
        GridLoadLevelUI.GetInstance().CloseLoadLevelButtons();
        LevelManager.levelNameLoad = levelName;
    }
    public void SetText(string newText) {


        txt.text = newText; 
    
    }


}
