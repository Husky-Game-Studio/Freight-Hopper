using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CollisionManagement))]
public class Gravity : MonoBehaviour
{
    private Rigidbody rb;
    private CollisionManagement collid;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        collid = GetComponent<CollisionManagement>();
    }

    public void OnEnable()
    {
        collid.CollisionDataCollected += ApplyGravity;
    }

    public void OnDisable()
    {
        collid.CollisionDataCollected -= ApplyGravity;
    }

    private void ApplyGravity()
    {
        if (collid.IsGrounded.current)
        {
            return;
        }

        rb.AddForce(CustomGravity.GetGravity(rb.position), ForceMode.Acceleration);
    }
}