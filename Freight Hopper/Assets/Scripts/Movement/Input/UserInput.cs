using UnityEngine;

public class UserInput : MonoBehaviour
{
    // Start is called before the first frame update
    private InputMaster master;

    private static UserInput input;

    public static UserInput Input => input;

    private bool groundPoundHeld;
    private bool jumpHeld;

    private void OnEnable()
    {
        master.Enable();
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
        }
        else
        {
            Destroy(this);
        }

        master = new InputMaster();
    }

    private void Update()
    {
        if (master.Player.GroundPound.triggered)
        {
            groundPoundHeld = !groundPoundHeld;
        }

        if (master.Player.Jump.triggered)
        {
            jumpHeld = !jumpHeld;
        }
    }

    // Returns the direction the player wants to move
    public Vector2 Move()
    {
        return master.Player.Movement.ReadValue<Vector2>();
    }

    // Returns the mouse movement for player looking
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