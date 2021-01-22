using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] Transform cameraLocation;
    // y min, y max
    [SerializeField] Vector2 yRotationLock;
    [SerializeField] Vector2 mouseSensitivity;
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LateUpdate()
    {
        FollowPlayer();
        RotatePlayer();
    }

    void FollowPlayer()
    {
        this.transform.position = cameraLocation.position;
    }

    void RotatePlayer()
    {
        Vector2 look = UserInput.Input.Look();
        Vector2 mouse = look * Time.deltaTime * mouseSensitivity;

        Vector2 rotation = new Vector2(mouse.x, Mathf.Clamp(mouse.y, yRotationLock.x, yRotationLock.y));
        Debug.Log("rotation: " + rotation);
        Quaternion quaternionRotation = new Quaternion(0, -rotation.y, rotation.x, 0).normalized;
        Debug.Log("Quaternion rotation: " + quaternionRotation);
        transform.eulerAngles += new Vector3(-rotation.y, rotation.x, 0);
    }
}