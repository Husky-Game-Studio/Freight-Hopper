using UnityEngine;

public class MovementBehavior : MonoBehaviour {
    private UserInput input;
    private Transform transform;

    private void Awake() {
        input = GetComponent<UserInput>();
        transform = GetComponent<Transform>();
    }

    private void FixedUpdate() {
        transform.Translate(new Vector3(input.Move().x, 0, input.Move().y));
    }
}