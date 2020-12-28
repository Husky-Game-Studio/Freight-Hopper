using UnityEngine;

public class MovementBehavior : MonoBehaviour {
    private UserInput input;
    private Transform transform;
    private Rigidbody rb;
    private Camera camera;
    private Transform cameraTransform;
    [SerializeField] private float speed;

    private void Awake() {
        speed = 10;
        input = GetComponent<UserInput>();
        transform = GetComponent<Transform>();
        camera = Camera.main;
        cameraTransform = camera.transform;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        Vector3 move = new Vector3(input.Move().x, 0f, input.Move().y);
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        cameraForward.Normalize();

        move = cameraForward * move.z + cameraRight * move.x;
        move.y = 0f;
        move.Normalize();
        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);
        /*transform.Translate(move * (Time.deltaTime * speed));*/
    }
}