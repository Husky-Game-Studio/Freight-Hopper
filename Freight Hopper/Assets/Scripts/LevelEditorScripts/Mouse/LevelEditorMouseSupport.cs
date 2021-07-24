using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace HGSLevelEditor
{
    public class LevelEditorMouseSupport : MonoBehaviour
    {
        //Variables 
        public Camera MainCamera = null;
        public GameObject targetCube;
        public UIManager transformSliders = null;
        GameObject selectedObject = null;

        void Update()
        {
            mouseSelectObject();
        }

        private void mouseSelectObject()
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Mouse is down");

                    RaycastHit hitInfo;

                    bool hit = Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, 1);

                    if (hit && hitInfo.transform.gameObject.name != "Floor")
                    {
                        //5 = UI layer 
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

        public void SetMovingObject(GameObject selected)
        {

            GameObject a = SetMovingObjectHelper(selected);
            transformSliders.SetSelectedObject(a);
        }

        public GameObject SetMovingObjectHelper(GameObject obj)
        {
            SetColor(obj);
            return selectedObject;
        }

        private void SetColor(GameObject selected)
        {
           
            if (selected != null)
            {
                selectedObject = selected;
                selectedObject.GetComponent<Outline>().OutlineWidth = 10.0f;
                selectedObject.GetComponent<Outline>().OutlineColor = Color.green;
                Debug.Log("Done!");

            }

            if (selected == null) {

                selectedObject.GetComponent<Outline>().OutlineWidth = 0.0f;
                selectedObject = selected;
            }
        }
    }
}

