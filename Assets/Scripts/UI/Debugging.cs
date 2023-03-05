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
    private int frameCount;
    private double dt;

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
            frameCount++;
            dt += Time.deltaTime;
            if (dt > refreshSpeed.Duration)
            {
                framesPerSecondText.text = "FPS: " + ((int)(frameCount / dt)).ToString();
                frameCount = 0;
                dt -= refreshSpeed.Duration;
            }
            refreshSpeed.Update(Time.deltaTime);
        }
    }

    private void UpdateText()
    {
        speedText.text = $"Speed: \t{movementBehavior.Speed.ToString("0.00")} m/s\n"+
                         $"HSpeed:\t{movementBehavior.HorizontalSpeed.ToString("0.00")} m/s\n"+
                         $"Pos:\t   {Player.Instance.transform.position}\n";
    }

    private void ToggleDebugging(InputAction.CallbackContext context)
    {
        debugGameobject.SetActive(!debugGameobject.activeSelf);
        F3Status = debugGameobject.activeSelf;
    }
}