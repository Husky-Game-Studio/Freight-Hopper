using UnityEngine;

public class MovementBehavior : MonoBehaviour {
    private UserInput input;
    private Transform transform;
    private Camera camera;
    private Transform cameraTransform;
    [SerializeField] private float speed;

    private void Awake() {
        speed = 10;
        input = GetComponent<UserInput>();
        transform = GetComponent<Transform>();
        camera = Camera.main;
        cameraTransform = camera.transform;
    }

    private void FixedUpdate() {
        Vector3 move = new Vector3(input.Move().x, 0f, input.Move().y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        move.Normalize();
        transform.Translate(move * (Time.deltaTime * speed));
    }
}