// for assert
using UnityEngine;
// for GUI elements: Button, Toggle
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseSupport : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject LevelSelector;
    public GameObject MainMenu;
    private UnityEngine.InputSystem.Mouse mouse;

    // Start is called before the first frame update
    private void Start()
    {
        mouse = UnityEngine.InputSystem.Mouse.current;
    }

    // Update is called once per frame
    private void Update()
    {
        if (mouse.leftButton.isPressed)
        {
            // Debug.Log("Mouse is down");
            if (!EventSystem.current.IsPointerOverGameObject()) // check for GUI
            {
                //Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hitInfo, Mathf.Infinity, 1);
                if (hit && hitInfo.collider.gameObject.name == "TempStartPlayP")
                {
                    LevelSelector.gameObject.SetActive(true);
                    MainMenu.gameObject.SetActive(false);
                }
                else if (hit && hitInfo.collider.gameObject.name == "TempStartQuitQ")
                {
                    Application.Quit();
                }
            }
        }
    }
}
