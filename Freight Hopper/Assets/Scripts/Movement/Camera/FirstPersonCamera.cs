using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    // I am not sure how you can edit mouse sensitivity by script using cinemachine
    //[SerializeField] private Vector2 mouseSensitivity;
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}