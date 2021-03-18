using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private Transform camTransform;
    [SerializeField] private Quaternion localRotation;
    private Transform player;

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
        localRotation = player.rotation;
    }

    private void LateUpdate()
    {
        FollowPlayer();
        RotatePlayer();
    }

    private void FollowPlayer()
    {
        camTransform.position = playerHead.position;
        localRotation *= (Quaternion.Inverse(camTransform.rotation) * player.rotation);
    }

    private void RotatePlayer()
    {
        // Up axis
        Vector3 upAxis = CustomGravity.GetUpAxis(player.position);

        // Get rotation

        Quaternion currentRotationPlayer = player.rotation;
        Vector2 mouse = UserInput.Input.Look() * mouseSensitivity * Time.deltaTime;
        Quaternion mouseRotationHorizontal = Quaternion.Euler(0, mouse.x, 0);
        Quaternion mouseRotationVertical = Quaternion.Euler(-mouse.y, 0, 0);

        //Var<Quaternion> delta = new Var(Quaternion.Inverse(camTransform.rotation) * Quaternion.FromToRotation(Vector3.up, upAxis));
        //delta *= mouseRotationHorizontal;
        //camTransform.rotation *= delta;

        //currentRotationPlayer *= mouseRotationHorizontal;

        //currentRotation += mouse;

        // Clamp it
        // rotation.y = Mathf.Clamp(rotation.y, yRotationLock.x, yRotationLock.y);

        // Rotate player horizontally
        // player.rotation *= mouseRotationHorizontal;
        //Quaternion.LookRotation(play);
        // Rotate camera vertically
        localRotation *= mouseRotationHorizontal;
        //localRotation *= mouseRotationVertical;

        //Quaternion currentRotationCamera = camTransform.rotation;
        // camTransform.rotation = player.rotation;
        // camTransform.rotation = currentRotationCamera * mouseRotationVertical;
        // camTransform.rotation = Quaternion.Euler(-rotation.y, camTransform.localEulerAngles.y, camTransform.localEulerAngles.z);
        camTransform.rotation = localRotation;
    }
}