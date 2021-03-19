using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    // Start is called before the first frame update
    private InputMaster master;

    private static UserInput input;

    public static UserInput Input => input;

    public delegate void PressEventHandler();

    public event PressEventHandler JumpInput;

    public event PressEventHandler JumpInputCanceled;

    public event PressEventHandler GroundPoundInput;

    public event PressEventHandler FullStopInput;

    public event PressEventHandler DashInput;

    public event PressEventHandler UpwardDashInput;

    [ReadOnly, SerializeField] private bool groundPoundHeld;
    [ReadOnly, SerializeField] private bool jumpHeld;

    private void OnEnable()
    {
        master.Enable();
        master.Player.GroundPound.performed += GroundPoundHeld;
        master.Player.Jump.performed += JumpHeld;
        master.Player.Jump.canceled += JumpReleased;
        master.Player.FullStop.performed += FullStopPressed;
    }

    private void OnDisable()
    {
        master.Disable();
    }

    private void Awake()
    {
        // Singleton instance pattern
        if (input == null)
        {
            input = this;
            master = new InputMaster();
        }
        else
        {
            Destroy(this);
        }
    }

    private void GroundPoundHeld(InputAction.CallbackContext context)
    {
        groundPoundHeld = !groundPoundHeld;
        if (groundPoundHeld)
        {
            GroundPoundInput?.Invoke();
        }
    }

    private void JumpHeld(InputAction.CallbackContext context)
    {
        jumpHeld = !jumpHeld;
        if (jumpHeld)
        {
            JumpInput?.Invoke();
        }
    }

    private void JumpReleased(InputAction.CallbackContext context)
    {
        JumpInputCanceled?.Invoke();
    }

    private void FullStopPressed(InputAction.CallbackContext context)
    {
        FullStopInput?.Invoke();
    }

    private void UpwardDashPressed(InputAction.CallbackContext context)
    {
        UpwardDashInput?.Invoke();
    }

    private void DashPressed(InputAction.CallbackContext context)
    {
        DashInput?.Invoke();
    }

    // Returns the direction the player wants to move
    public Vector2 Move()
    {
        return master.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 Look()
    {
        return master.Player.Look.ReadValue<Vector2>();
    }

    // Returns if the player jumped
    public bool Jump()
    {
        return jumpHeld;
    }

    // Returns true if the player press the restart key/button
    public bool Restart()
    {
        return master.Player.Restart.triggered;
    }

    // Returns true if player presses the dash key/button
    public bool Dash()
    {
        return master.Player.Dash.triggered;
    }

    // Returns true if player presses the FullStop key/button
    public bool FullStopTriggered()
    {
        return master.Player.FullStop.triggered;
    }

    // Returns true if player presses the UpwardDash key/button
    public bool UpwardDashTriggered()
    {
        return master.Player.UpwardDash.triggered;
    }

    // Returns true if player presses the GrapplePole key/button
    public bool GrapplePole()
    {
        return master.Player.GrapplePole.triggered;
    }

    // Returns true if player is holding the GroundPound key/button down
    public bool GroundPoundTriggered()
    {
        return groundPoundHeld;
    }
}