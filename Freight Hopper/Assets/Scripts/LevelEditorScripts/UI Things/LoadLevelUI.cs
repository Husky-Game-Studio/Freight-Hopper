using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using HGSLevelEditor;

public class LoadLevelUI : MonoBehaviour
{
    [SerializeField]
    private Text txt;
    public string levelName;

    public void LoadLevel() {


        SaveLoadLevel.GetInstance().LoadButton(levelName);
        MenuOpener.GetInstance().CloseLoadLevelButtons();
    
    }
    public void SetText(string newText) {


        txt.text = newText; 
    
    }


}
