using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UserInput : MonoBehaviour
{
    public InputMaster UserInputMaster => master;
    private InputMaster master;

    private static UserInput input;

    public static UserInput Instance => input;

    public delegate void PressEventHandler();

    public event PressEventHandler JumpInput;

    public event PressEventHandler JumpInputCanceled;

    public event PressEventHandler GroundPoundInput;

    public event PressEventHandler GroundPoundCanceled;

    public event PressEventHandler FullStopInput;

    public event PressEventHandler BurstInput;

    public event PressEventHandler GrappleInput;

    public event PressEventHandler UpwardDashInput;

    public event PressEventHandler UpwardDashInputCanceled;

    [ReadOnly, SerializeField] private bool groundPoundHeld;
    [ReadOnly, SerializeField] private bool jumpHeld;
    [ReadOnly, SerializeField] private bool upwardDashHeld;

    private void OnEnable()
    {
        master.Enable();
        master.Player.GroundPound.performed += GroundPoundHeld;
        master.Player.GroundPound.canceled += GroundPoundReleased;
        master.Player.UpwardDash.performed += UpwardDashHeld;
        master.Player.UpwardDash.canceled += UpwardDashReleased;
        master.Player.Jump.performed += JumpHeld;
        master.Player.Jump.canceled += JumpReleased;
        master.Player.FullStop.performed += FullStopPressed;
        master.Player.Burst.performed += BurstPressed;
        master.Player.GrapplePole.performed += GrapplePressed;
        if (SceneManager.GetActiveScene().name.Equals("DefaultScene"))
        {
            LevelController.LevelLoadedIn += RespawnLinked;
        }
        else
        {
            Player.PlayerLoadedIn += RespawnLinked;
        }
    }

    private void RespawnLinked()
    {
        master.Player.Restart.performed += LevelController.Instance.Respawn;
    }

    private void OnDisable()
    {
        master.Disable();
        LevelController.LevelLoadedIn -= RespawnLinked;
        master.Player.Restart.performed -= LevelController.Instance.Respawn;
        Player.PlayerLoadedIn -= RespawnLinked;
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

    private void UpwardDashHeld(InputAction.CallbackContext context)
    {
        upwardDashHeld = !upwardDashHeld;
        if (upwardDashHeld)
        {
            UpwardDashInput?.Invoke();
        }
    }

    private void UpwardDashReleased(InputAction.CallbackContext context)
    {
        UpwardDashInputCanceled?.Invoke();
    }

    private void GroundPoundReleased(InputAction.CallbackContext context)
    {
        GroundPoundCanceled?.Invoke();
    }

    private void GrapplePressed(InputAction.CallbackContext context)
    {
        GrappleInput?.Invoke();
    }

    private void FullStopPressed(InputAction.CallbackContext context)
    {
        FullStopInput?.Invoke();
    }

    private void UpwardDashPressed(InputAction.CallbackContext context)
    {
        UpwardDashInput?.Invoke();
    }

    private void BurstPressed(InputAction.CallbackContext context)
    {
        BurstInput?.Invoke();
    }

    // Returns the direction the player wants to move
    public Vector3 Move()
    {
        Vector2 rawInput = master.Player.Movement.ReadValue<Vector2>();
        return new Vector3(rawInput.x, 0, rawInput.y);
    }

    public Vector2 Look()
    {
        return master.Player.Look.ReadValue<Vector2>();
    }

    // Returns true if the player press the restart key/button
    public bool Restart()
    {
        return master.Player.Restart.triggered;
    }
}