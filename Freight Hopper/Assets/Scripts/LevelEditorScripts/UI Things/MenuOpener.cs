using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOpener : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject menu;
    public Button exit;

    private void Start()
    {
        Button x = exit.GetComponent<Button>();
        x.onClick.AddListener(ExitOnClick); 
    }

    public void OpenMenu() {

        if (menu != null) {

            menu.SetActive(true); 
   
        }
    }

    public void PlayLevel() {

        SceneManager.LoadScene("CustomLevel 0 0", LoadSceneMode.Single);
        
    }
    void ExitOnClick() {

        menu.SetActive(false);
    
    }
}
