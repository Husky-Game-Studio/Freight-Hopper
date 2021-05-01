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
    }

    private void RotatePlayer()
    {
        // Up axis
        Vector3 upAxis = player.GetComponent<CollisionManagement>().ValidUpAxis;

        // convert input to rotation
        Vector2 mouse = UserInput.Instance.Look() * mouseSensitivity * Time.deltaTime;
        Quaternion mouseRotationHorizontal = Quaternion.Euler(0, mouse.x, 0);
        Quaternion mouseRotationVertical = Quaternion.Euler(-mouse.y, 0, 0);

        localRotation *= mouseRotationVertical;

        // Apply camera and player rotation
        camTransform.rotation = Quaternion.LookRotation(player.forward, upAxis) * localRotation;
        Vector3 forward = camTransform.forward.ProjectOnContactPlane(upAxis).normalized;
        player.LookAt(player.position + forward, upAxis);
        player.rotation *= mouseRotationHorizontal;
    }

    public void RotateTo(Quaternion rotation)
    {
        Vector3 upAxis = player.GetComponent<CollisionManagement>().ValidUpAxis;
        Vector3 forward = (rotation * player.forward).ProjectOnContactPlane(upAxis).normalized;
        player.LookAt(player.position + forward, upAxis);
    }
}