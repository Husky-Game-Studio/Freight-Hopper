using System; // for assert
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for GUI elements: Button, Toggle
using UnityEngine.EventSystems;

public class MouseSupport : MonoBehaviour
{
    public Camera MainCamera = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("Mouse is down");
            if (!EventSystem.current.IsPointerOverGameObject()) // check for GUI
            {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, 1);
                if (hit && hitInfo.collider.gameObject.name == "Sphere")
                {
                    
                }
            }
        }
    }
}
