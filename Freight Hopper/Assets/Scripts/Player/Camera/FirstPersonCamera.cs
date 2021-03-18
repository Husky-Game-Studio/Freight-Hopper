using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private Transform camTransform;
    private Transform player;
    private Vector2 rotation;

    [SerializeField] private Transform playerHead;

    // y min, y max
    [SerializeField] private Vector2 yRotationLock = new Vector2(-89.999f, 89.999f);

    [SerializeField] private Vector2 mouseSensitivity;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        camTransform = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rotation = new Vector2(camTransform.eulerAngles.x, camTransform.eulerAngles.y);
    }

    private void LateUpdate()
    {
        FollowPlayer();
        RotatePlayer();
    }

    private void FollowPlayer()
    {
        camTransform.position = playerHead.position;
    }

    private void RotatePlayer()
    {
        Vector2 mouse = UserInput.Input.Look() * mouseSensitivity * Time.deltaTime;
        rotation += mouse;
        rotation.y = Mathf.Clamp(rotation.y, yRotationLock.x, yRotationLock.y);

        player.rotation = Quaternion.Euler(0, rotation.x, 0);
        camTransform.rotation = player.rotation;
        camTransform.rotation = Quaternion.Euler(-rotation.y, camTransform.localEulerAngles.y, camTransform.localEulerAngles.z);
    }
}