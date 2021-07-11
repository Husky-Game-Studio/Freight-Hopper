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

       // SceneManager.LoadScene("LevelEditorBeginning");
        SaveLoadLevel.GetInstance().LoadButton(levelName, true);
        GridLoadLevelUI.GetInstance().CloseLoadLevelButtons();
        LevelManager.levelNameLoad = levelName;
    }
    public void SetText(string newText) {


        txt.text = newText; 
    
    }


}
