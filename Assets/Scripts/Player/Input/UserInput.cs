using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public bool GroundPoundHeld => groundPoundHeld;
    public bool JumpHeld => jumpHeld;

    [ReadOnly, SerializeField] private bool groundPoundHeld;
    [ReadOnly, SerializeField] private bool jumpHeld;
    void Awake()
    {
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
    void OnEnable()
    {
        master.Player.GroundPound.performed += GroundPoundPressed;
        master.Player.GroundPound.canceled += GroundPoundReleased;
        master.Player.Jump.performed += JumpPressed;
        master.Player.Jump.canceled += JumpReleased;

        master.Player.Restart.performed += LevelController.Instance.Respawn;
    }

    private void OnDisable()
    {
        master.Disable();
        master.Player.GroundPound.performed -= GroundPoundPressed;
        master.Player.GroundPound.canceled -= GroundPoundReleased;
        master.Player.Jump.performed -= JumpPressed;
        master.Player.Jump.canceled -= JumpReleased;
        master.Player.Restart.performed -= LevelController.Instance.Respawn;
        master.Dispose();
    }

    private int frameCount;
    private bool masterEnabled;
    private void Update()
    {
        if (frameCount >= 4)
        {
            master.Enable();
            masterEnabled = true;
            frameCount = 0;
        } else if (!masterEnabled){
            frameCount++;
        }
    }


    private void GroundPoundPressed(InputAction.CallbackContext context)
    {
        if(groundPoundHeld)
        {
            return;
        }
        groundPoundHeld = true;
        GroundPoundInput?.Invoke();
    }

    private void JumpPressed(InputAction.CallbackContext _)
    {
        if(jumpHeld)
        {
            return;
        }
        jumpHeld = true;
        JumpInput?.Invoke();
    }

    private void JumpReleased(InputAction.CallbackContext _)
    {
        if (!jumpHeld)
        {
            return;
        }
        jumpHeld = false;
        JumpInputCanceled?.Invoke();
    }

    private void GroundPoundReleased(InputAction.CallbackContext _)
    {
        if (!groundPoundHeld)
        {
            return;
        }
        groundPoundHeld = false;
        GroundPoundCanceled?.Invoke();
    }

    // Returns the direction the player wants to move
    public Vector3 Move()
    {
        Vector2 rawInput = master.Player.Movement.ReadValue<Vector2>();
        return new Vector3(rawInput.x, 0, rawInput.y);
    }

    // Returns true if the player press the restart key/button
    public bool Restart()
    {
        return master.Player.Restart.triggered;
    }
}