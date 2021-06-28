using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HGSLevelEditor;
public class Exit : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {

            SceneManager.LoadScene("LevelEditorBeginning");
        }

       
    }

    public void openNew()
    {
        SceneManager.LoadScene("LevelEditorBeginning");
        LevelManager.levelNameLoad = null; 
    }
}
