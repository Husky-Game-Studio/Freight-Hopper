using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
// Classes that inherit from this must call gravity loop on their own loops
public abstract class Gravity : MonoBehaviour
{
    [SerializeField] protected bool useGravity = true;
    protected Rigidbody rb;

    private void Awake()
    {
        InitializeComponents();
    }

    protected virtual void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void DisableGravity()
    {
        useGravity = false;
    }

    public void EnableGravity()
    {
        useGravity = true;
    }

    protected virtual void GravityLoop()
    {
        if (!useGravity)
        {
            return;
        }
        ApplyGravityForce();
    }

    protected void ApplyGravityForce()
    {
        rb.AddForce(CustomGravity.GetGravity(rb.position), ForceMode.Acceleration);
    }
}