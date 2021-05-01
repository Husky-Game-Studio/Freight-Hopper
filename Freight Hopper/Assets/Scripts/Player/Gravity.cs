using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CollisionManagement))]
public class Gravity : MonoBehaviour
{
    [SerializeField] private bool useGravity = true;
    private Rigidbody rb;
    private CollisionManagement collision;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        collision = GetComponent<CollisionManagement>();
    }

    public void OnEnable()
    {
        collision.CollisionDataCollected += ApplyGravity;
    }

    public void OnDisable()
    {
        collision.CollisionDataCollected -= ApplyGravity;
    }

    public void DisableGravity()
    {
        useGravity = false;
    }

    public void EnableGravity()
    {
        useGravity = true;
    }

    private void ApplyGravity()
    {
        if (!useGravity || collision.IsGrounded.current)
        {
            return;
        }

        rb.AddForce(CustomGravity.GetGravity(rb.position), ForceMode.Acceleration);
    }
}