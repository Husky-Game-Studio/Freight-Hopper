using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private Transform camTransform;
    [SerializeField, ReadOnly] private Quaternion localRotation;
    private Transform player;
    [SerializeField, ReadOnly] private Vector3 upAxisAngleRotation;
    [SerializeField, ReadOnly] private Var<Vector3> upAxis;
    [SerializeField, ReadOnly] private Vector3 smoothedUpAxis;
    [SerializeField, ReadOnly] private Vector3 oldUpAxis;
    [SerializeField, ReadOnly] private float timeStep;
    [SerializeField] private Transform playerHead;

    // y min, y max
    [SerializeField] private Vector2 yRotationLock = new Vector2(-89.999f, 89.999f);

    [SerializeField] private Vector2 mouseSensitivity;

    // for when the cameras up axis changes like for gravity or wall running
    [SerializeField] private float smoothingDelta;

    private CollisionManagement playerCM;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        camTransform = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        localRotation = player.rotation;
        playerCM = player.GetComponent<PhysicsManager>().collisionManager;
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
        CalculateSmoothedUpAxis();

        // convert input to rotation
        Vector2 mouse = UserInput.Instance.Look() * mouseSensitivity * Time.deltaTime;
        Quaternion mouseRotationHorizontal = Quaternion.Euler(0, mouse.x, 0);
        Quaternion mouseRotationVertical = Quaternion.Euler(-mouse.y, 0, 0);

        // Just prevents issues with camera glitching at the poles
        float verticalAngle = (localRotation * mouseRotationVertical).eulerAngles.x;
        if ((localRotation * mouseRotationVertical).eulerAngles.y != 180 && (verticalAngle < yRotationLock.x || verticalAngle > yRotationLock.y))
        {
            localRotation *= mouseRotationVertical;
        }
        // Apply camera and player rotation
        camTransform.rotation = Quaternion.LookRotation(player.forward, smoothedUpAxis) * localRotation;
        Vector3 forward = camTransform.forward.ProjectOnContactPlane(smoothedUpAxis).normalized;
        player.LookAt(player.position + forward, playerCM.ValidUpAxis);
        player.rotation *= mouseRotationHorizontal;
    }

    private void CalculateSmoothedUpAxis()
    {
        upAxis.current = Quaternion.Euler(upAxisAngleRotation) * playerCM.ValidUpAxis;

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