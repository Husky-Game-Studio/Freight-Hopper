using UnityEngine;

public class UserInput : MonoBehaviour {

    // Start is called before the first frame update
    private InputMaster master;

    private void OnEnable() {
        master.Enable();
    }

    private void OnDisable() {
        master.Disable();
    }

    private void Awake() {
        master = new InputMaster();
    }

    public Vector2 Move() {
        return master.Player.Movement.ReadValue<Vector2>();
    }

    public bool Jumped() {
        return master.Player.Jump.triggered;
    }
}