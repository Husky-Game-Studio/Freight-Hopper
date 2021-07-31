using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectUI : MonoBehaviour
{
    public Transform objectButtons;
    public GameObject objectButtonPrefab;

    public ObjectCollection eObj; 

    // Start is called before the first frame update
    void Start()
    {
        CreateObjectButtons();
    }

    public void CreateObjectButtons() {

        foreach (LevelEditorObjects obj in eObj.objects) {

            GameObject button = Instantiate(objectButtonPrefab);
            button.transform.SetParent(objectButtons);

            ButtonLoad newButton = button.GetComponent<ButtonLoad>();
            newButton.spawnObject = obj.objectPrefab;
            newButton.objectPhoto = obj.buttonPhoto;
            newButton.objectID = obj.objectID;
            
        }
    }
}
