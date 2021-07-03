 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorMouseSupport : MonoBehaviour
{

    public Camera MainCamera = null;
    public GameObject targetCube; 

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

        if (Input.GetMouseButtonDown(0)) {

            Debug.Log("Mouse is down");

            RaycastHit hitInfo = new RaycastHit();

            bool hit = Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, 1);

            if (hit && hitInfo.transform.gameObject.name != "Floor")
            {
                Debug.Log("Object selected: " + hitInfo.transform.gameObject.name);
                SelectObject(hitInfo.transform.gameObject);
            }

            else {

                SelectObject(null);
            }

        }
   
    }

    void SelectObject(GameObject selected) {

        //targetCube.transform.position = selected.transform.position;

        selected.transform.position = targetCube.transform.position;

      
    }




}
