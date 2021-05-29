using UnityEngine;

[System.Serializable]
public class Gravity
{
    [SerializeField] private bool useGravity = true;
    [SerializeField] private bool aerial = false;
    private Rigidbody rb;
    private CollisionManagement collisionManagement;

    public void Initialize(Rigidbody rb, CollisionManagement collisionManagement)
    {
        this.rb = rb;
        this.collisionManagement = collisionManagement;
        rb.useGravity = false;
        collisionManagement.CollisionDataCollected += GravityLoop;
    }

    ~Gravity()
    {
        collisionManagement.CollisionDataCollected -= GravityLoop;
    }

    public void DisableGravity()
    {
        useGravity = false;
    }

    public void EnableGravity()
    {
        useGravity = true;
    }

    private void GravityLoop()
    {
        if (!useGravity || (collisionManagement.IsGrounded.current && !aerial))
        {
            return;
        }
        rb.AddForce(CustomGravity.GetGravity(rb.position), ForceMode.Acceleration);
    }
}