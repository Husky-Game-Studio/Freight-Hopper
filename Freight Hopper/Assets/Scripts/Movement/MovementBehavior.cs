using UnityEngine;

public class MovementBehavior : MonoBehaviour {
    private UserInput input;
    private Transform transform;
    [SerializeField] private float speed;

    private void Awake() {
        speed = 10;
        input = GetComponent<UserInput>();
        transform = GetComponent<Transform>();
    }

    private void FixedUpdate() {
        transform.Translate(new Vector3(input.Move().x, 0, input.Move().y) * Time.deltaTime * speed);
    }
}