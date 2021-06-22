using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOpener : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject menu;
    public Button exit;

    private void Start()
    {
        Button x = exit.GetComponent<Button>();
        x.onClick.AddListener(exitOnClick); 
    }

    public void OpenMenu() {

        if (menu != null) {

            menu.SetActive(true); 
   
        }
    }

    public void playLevel() {

        SceneManager.LoadScene("CustomLevel", LoadSceneMode.Single);
        
    }
    void exitOnClick() {

        menu.SetActive(false);
    
    }
}
