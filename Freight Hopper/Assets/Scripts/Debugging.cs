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

    [SerializeField] private AutomaticTimer refreshSpeed = new AutomaticTimer(0.2f);

    private static bool F3Status;

    private void OnEnable()
    {
        UserInput.Instance.UserInputMaster.Player.Debug.performed += ToggleDebugging;
        debugGameobject.SetActive(F3Status);
        refreshSpeed.Subscribe(UpdateText);
    }

    private void OnDisable()
    {
        UserInput.Instance.UserInputMaster.Player.Debug.performed -= ToggleDebugging;
        refreshSpeed.Unsubscribe(UpdateText);
    }

    private void Update()
    {
        if (debugGameobject.activeSelf)
        {
            refreshSpeed.Update(Time.deltaTime);
        }
    }

    private void UpdateText()
    {
        framesPerSecondText.text = "FPS: " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
        speedText.text = "Speed: " + movementBehavior.Speed.ToString("0.00") + " m/s";
        horizontalSpeedText.text = "HSpeed: " + movementBehavior.HorizontalSpeed.ToString("0.00") + " m/s";
    }

    private void ToggleDebugging(InputAction.CallbackContext context)
    {
        debugGameobject.SetActive(!debugGameobject.activeSelf);
        F3Status = debugGameobject.activeSelf;
    }
}