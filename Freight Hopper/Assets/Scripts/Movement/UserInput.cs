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
    public bool Jumped()
    {
        return master.Player.Jump.triggered;
    }

    // Returns true if the player press the restart key/button
    public bool Restart()
    {
        return master.Player.Restart.triggered;
    }
}