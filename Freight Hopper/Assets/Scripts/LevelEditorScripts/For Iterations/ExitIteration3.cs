using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HGSLevelEditor;
public class ExitIteration3 : MonoBehaviour
{
    [SerializeField] private GameObject validatingMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene("LevelEditorIteration3");
        }

    }
    public void OpenNew()
    {
        Debug.Log("Level Save: " + LevelManager.levelNameLoad);

        if (LevelManager.levelNameLoad == null || LevelManager.levelNameLoad == "temp")
        {
            validatingMenu.SetActive(true);
        }
        else {
            SceneManager.LoadScene("LevelEditorIteration3");
            LevelManager.levelNameLoad = null; 
        }
       
    }

    public void SetNull() {

        SetInactive();
        LevelManager.levelNameLoad = null;
        SceneManager.LoadScene("LevelEditorIteration3");

    }

    public void SetInactive() {

        validatingMenu.SetActive(false);
        LoadLevelUI.levelSelect = null;
    }
}
