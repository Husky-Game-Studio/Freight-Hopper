using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedLevel : MonoBehaviour
{
    // Start is called before the first frame update
    public Text selectedLevel;

    private void Update()
    {
        selectedLevel.text = "Level Selected: " + LoadLevelUI.levelSelect; 
    }
}
