using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HGSLevelEditor;
public class ExitIteration2 : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene("LevelEditorIteration2");
        }


    }

    public void openNew()
    {
        SceneManager.LoadScene("LevelEditorIteration2");
        LevelManager.levelNameLoad = null;
    }
}

