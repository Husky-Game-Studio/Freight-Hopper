using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//I don't think that I am using this -- this can be smited 
public class DefaultMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) {

            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
