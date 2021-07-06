 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems;
namespace HGSLevelEditor { 
public class LevelEditorMouseSupport : MonoBehaviour
{

    public Camera MainCamera = null;
    public GameObject targetCube;
    public UIManager transformSliders = null; 
    GameObject selectedObject = null;
    private Color selectedColor = Color.green;
    private Color objColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(MainCamera != null);
    }

    // Update is called once per frame
    void Update()
    {
        mouseSelectObject();
    }

    void mouseSelectObject() {

            if (Input.GetMouseButtonDown(0))
            {

                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Mouse is down");

                    RaycastHit hitInfo = new RaycastHit();

                    bool hit = Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, 1);

                    if (hit && hitInfo.transform.gameObject.name != "Floor")
                    {
                        if (hitInfo.transform.gameObject.layer != 5)
                        {
                            Debug.Log("Object selected: " + hitInfo.transform.gameObject.name);
                            SetMovingObject(hitInfo.transform.gameObject);
                        }
                    }

                    else
                    {
                        SetMovingObject(null);
                    }
                }
            }
    }

     public void SetMovingObject(GameObject selected) {

            GameObject a = SetMovingObjectHelper(selected);
          
            transformSliders.SetSelectedObject(a);
    }

        public GameObject SetMovingObjectHelper(GameObject obj) {

            SetColor(obj);
            return selectedObject; 
        
        }

        void SetColor(GameObject selected) {

            if (selectedObject != null)
            {
                selectedObject.GetComponent<Renderer>().material.color = objColor;
            }

            selectedObject = selected;

            if (selectedObject != null)
            {
                objColor = selected.GetComponent<Renderer>().material.color;
                selectedObject.GetComponent<Renderer>().material.color = selectedColor;

            }
        }
    }
}
    
