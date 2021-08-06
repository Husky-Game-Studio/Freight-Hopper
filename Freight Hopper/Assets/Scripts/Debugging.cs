using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Debugging : MonoBehaviour
{
    [SerializeField] private GameObject debugGameobject;
    [SerializeField] private TextMeshProUGUI framesPerSecondText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI horizontalSpeedText;

    [SerializeField] private MovementBehavior movementBehavior;
    [SerializeField] private Timer refreshSpeed;

    private static bool F3Status;

    private void OnEnable()
    {
        UserInput.Instance.UserInputMaster.Player.Debug.performed += ToggleDebugging;
        debugGameobject.SetActive(F3Status);
        refreshSpeed.ResetTimer();
    }

    private void OnDisable()
    {
        UserInput.Instance.UserInputMaster.Player.Debug.performed -= ToggleDebugging;
    }

    private void Update()
    {
        if (debugGameobject.activeSelf)
        {
            if (!refreshSpeed.TimerActive())
            {
                refreshSpeed.ResetTimer();

                framesPerSecondText.text = "FPS: " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
                speedText.text = "Speed: " + movementBehavior.Speed.ToString("0.00") + " m/s";
                horizontalSpeedText.text = "HSpeed: " + movementBehavior.HorizontalSpeed.ToString("0.00") + " m/s";
            }

            refreshSpeed.CountDown(Time.deltaTime);
        }
    }

    private void ToggleDebugging(InputAction.CallbackContext context)
    {
        debugGameobject.SetActive(!debugGameobject.activeSelf);
        F3Status = debugGameobject.activeSelf;
    }
}