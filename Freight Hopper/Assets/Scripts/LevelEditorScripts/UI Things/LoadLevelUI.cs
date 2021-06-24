using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using HGSLevelEditor;

public class LoadLevelUI : MonoBehaviour
{
    public Text txt;
    public string levelName;

    public void LoadLevel() {


        SaveLoadLevel.GetInstance().LoadButton(levelName);
        MenuOpener.GetInstance().CloseLoadLevelButtons();
    
    }


}
