using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class GridObjectUI : MonoBehaviour
{
    public Transform objectButtons;
    public GameObject objectButtonPrefab;

    private ObjectCollection eObj = null;
    public ListObjectCollection allObjectLists; 

    public Dropdown objectDropdown;

    enum objectType { 
    
      everything = 0, 
      rocks = 1, 
      trainCarts = 2,
      interactables = 3
       
    }
    // Start is called before the first frame update
    void Start()
    {
        SetObjectButtons(0);
        objectDropdown.onValueChanged.AddListener(SetObjectButtons);
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

    public void SetObjectButtons(int value) {
    
        //Delete current object buttons
        if (objectButtons.transform.childCount != 0) {

            for (int i = 0; i < objectButtons.transform.childCount; i++)
            {
                Destroy(objectButtons.transform.GetChild(i).gameObject);
            }
        }
    
        if (objectDropdown.value == (int)objectType.everything) {
            SearchObjectList("Objects");
        }
        if (objectDropdown.value == (int)objectType.rocks) {
            SearchObjectList("Rocks");
        }
        if (objectDropdown.value == (int)objectType.trainCarts) {
            SearchObjectList("Train Carts");
        }
        if (objectDropdown.value == (int)objectType.interactables) {
            SearchObjectList("Interactables"); 
        }
        
    }

    public void SearchObjectList(string listName) {

        foreach (ObjectCollection obj in allObjectLists.objects)
        {
            if (obj.name == listName) {

                eObj = obj; 
            }

        }
        CreateObjectButtons();

    }
}
