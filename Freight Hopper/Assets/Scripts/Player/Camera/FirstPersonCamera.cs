using System.Collections;
using Cinemachine;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField, ReadOnly] private Vector3 upAxisAngleRotation = Vector3.zero;
    [SerializeField, ReadOnly] private Memory<Vector3> upAxis;
    [SerializeField, ReadOnly] private Vector3 smoothedUpAxis;
    [SerializeField, ReadOnly] private Vector3 oldUpAxis;
    [SerializeField, ReadOnly] private float timeStep;
    [SerializeField] private Transform upAxisTransform;
    private Transform player;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    [SerializeField] private GameObject ui;
    private GameObject otherUIs;
    [SerializeField, ReadOnly] private bool freecam = false;

    // for when the cameras up axis changes like for gravity or wall running
    [SerializeField] private float smoothingDelta;
    [SerializeField] private float mouseSensitivityConversionValue = 10;
    int curFrameCount = 0;
    const int stopCameraFrameCount = 4;
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cinemachineVirtualCamera.m_Lens.FieldOfView = Settings.GetFOV();
        Vector2 mouseSensitivity = Settings.GetMouseSensitivity();
        player = Player.Instance.transform.Find("Mesh");
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = mouseSensitivity.x / mouseSensitivityConversionValue;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = mouseSensitivity.y / mouseSensitivityConversionValue;

    }

    private void Update()
    {
        CalculateSmoothedUpAxis(Vector3.up);
        if((Time.timeScale == 0 || stopCameraFrameCount > curFrameCount) && !freecam)
        {
            cinemachineVirtualCamera.gameObject.SetActive(false);
        } else {
            cinemachineVirtualCamera.gameObject.SetActive(true);
        }
        curFrameCount++;
        
    }

    private void FixedUpdate()
    {
        Vector3 forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        player.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    private void OnEnable()
    {
        UserInput.Instance.UserInputMaster.Player.Freecam.performed += ToggleFreecam;
    }
    private void OnDisable()
    {
        UserInput.Instance.UserInputMaster.Player.Freecam.performed -= ToggleFreecam;
    }

    void ToggleFreecam(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if(Time.timeScale == 0 && !freecam){
            return;
        }
        freecam = !freecam;
        if(freecam) {
            Time.timeScale = 0;
            ui.SetActive(false);
            otherUIs = GameObject.Find("UniqueUICanvas");
            if(otherUIs != null){
                otherUIs.SetActive(false);
            }
            
        }
        else {
            Time.timeScale = 1;
            ui.SetActive(true);
            if (otherUIs != null)
            {
                otherUIs.SetActive(true);
            }
            
        }
    }

    private void CalculateSmoothedUpAxis(Vector3 upAxisCamera)
    {
        upAxis.current = Quaternion.Euler(upAxisAngleRotation) * upAxisCamera;
        
        if (upAxis.current != upAxis.old)
        {
            timeStep = 0;
            oldUpAxis = upAxis.old;
        }
        upAxis.UpdateOld();
        timeStep = Mathf.Min(timeStep + smoothingDelta, 1);
        smoothedUpAxis = Vector3.Lerp(oldUpAxis, upAxis.current, timeStep);
        upAxisTransform.transform.up = smoothedUpAxis;
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