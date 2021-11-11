using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private Transform camTransform;
    [SerializeField, ReadOnly] private Quaternion localRotation;
    private Transform player;
    [SerializeField, ReadOnly] private Vector3 upAxisAngleRotation;
    [SerializeField, ReadOnly] private Memory<Vector3> upAxis;
    [SerializeField, ReadOnly] private Vector3 smoothedUpAxis;
    [SerializeField, ReadOnly] private Vector3 oldUpAxis;
    [SerializeField, ReadOnly] private float timeStep;
    [SerializeField] private Transform playerHead;
    public static int fov;
    // y min, y max
    [SerializeField] private float yRotationLock = 90;

    public static Vector2 mouseSensitivity;

    // for when the cameras up axis changes like for gravity or wall running
    [SerializeField] private float smoothingDelta;

    // This is to stop the input while the level is loading. At least if it was implemented
    private bool cameraEnabled = false;

    private CollisionManagement playerCM;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Camera.main.fieldOfView = fov;
        camTransform = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        localRotation = camTransform.rotation;
        playerCM = player.GetComponent<PhysicsManager>().collisionManager;

        // Need to make this false until level loaded in the future :(
        cameraEnabled = true;
    }

    private void Update()
    {
        FollowPlayer();
    }

    private void FixedUpdate()
    {
        if (cameraEnabled)
        {
            RotatePlayer();
        }
    }

    private void FollowPlayer()
    {
        camTransform.position = playerHead.position;
    }

    private void RotatePlayer()
    {
        Vector3 validUpAxis = playerCM.ValidUpAxis;

        CalculateSmoothedUpAxis(validUpAxis);

        // convert input to rotation
        Vector2 mouse = UserInput.Instance.Look() * mouseSensitivity * Time.deltaTime;

        Quaternion mouseRotationHorizontal = Quaternion.AngleAxis(mouse.x, Vector3.up);
        Quaternion mouseRotationVertical = Quaternion.AngleAxis(mouse.y, -Vector3.right);

        Quaternion axisChange = Quaternion.LookRotation(player.forward, smoothedUpAxis);

        Quaternion nextRotation = Quaternion.LookRotation(player.forward, smoothedUpAxis) * localRotation * mouseRotationVertical;

        if (Quaternion.Angle(nextRotation, axisChange) < yRotationLock)
        {
            localRotation *= mouseRotationVertical;
        }

        camTransform.rotation = Quaternion.LookRotation(player.forward, smoothedUpAxis) * localRotation;

        Vector3 forward = camTransform.forward.ProjectOnContactPlane(smoothedUpAxis).normalized;
        player.LookAt(player.position + forward, validUpAxis);

        player.rotation *= mouseRotationHorizontal;
    }

    private void CalculateSmoothedUpAxis(Vector3 upAxisCamera)
    {
        upAxis.current = Quaternion.Euler(upAxisAngleRotation) * upAxisCamera;

        if (upAxis.current != upAxis.old)
        {
            timeStep = 0;
            oldUpAxis = upAxis.old;
            smoothedUpAxis = upAxis.old;
        }
        upAxis.UpdateOld();
        timeStep = Mathf.Min(timeStep + smoothingDelta, 1);
        smoothedUpAxis = Vector3.Lerp(oldUpAxis, upAxis.current, timeStep);
        if (timeStep >= 1)
        {
            oldUpAxis = upAxis.current;
            smoothedUpAxis = upAxis.current;
        }
    }

    public void TiltUpAxis(Vector3 angles)
    {
        upAxisAngleRotation = angles;
    }

    public void ResetUpAxis()
    {
        upAxisAngleRotation = Vector3.zero;
    }
}