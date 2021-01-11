using UnityEngine;

public class UserInput : MonoBehaviour
{
    // Start is called before the first frame update
    private InputMaster master;

    private static UserInput input;

    public static UserInput Input => input;

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

    // Returns the direction the player wants to move
    public Vector2 Move()
    {
        return master.Player.Movement.ReadValue<Vector2>();
    }

    // Returns if the player jumped
    public bool Jump()
    {
        return master.Player.Jump.triggered;
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
    public bool FullStop()
    {
        return master.Player.FullStop.triggered;
    }

    // Returns true if player presses the UpwardDash key/button
    public bool UpwardDash()
    {
        return master.Player.UpwardDash.triggered;
    }

    // Returns true if player presses the GrapplePole key/button
    public bool GrapplePole()
    {
        return master.Player.GrapplePole.triggered;
    }

    // Returns true if player presses the GroundPound key/button
    public bool GroundPound()
    {
        return master.Player.GroundPound.triggered;
    }
}