using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private Transform cameraLocation;
    // y min, y max
    [SerializeField] Vector2 yRotationLock;
    [SerializeField] Vector2 mouseSensitivity;
    private Vector2 rotation;
    private void Awake()
    {
        cameraLocation = transform.parent;
        rotation = new Vector2(transform.eulerAngles.x, transform.eulerAngles.y);
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
        Vector2 mouse = UserInput.Input.Look() * mouseSensitivity * Time.deltaTime;

        rotation += mouse;
        rotation.y = Mathf.Clamp(rotation.y, yRotationLock.x, yRotationLock.y);

        transform.localRotation = Quaternion.Euler(-rotation.y, 0, transform.localEulerAngles.z);
        transform.parent.rotation = Quaternion.Euler(0, rotation.x, 0);
    }
}