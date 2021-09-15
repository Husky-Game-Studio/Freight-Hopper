using UnityEngine;

[System.Serializable]
public class Gravity
{
    [SerializeField] private bool useGravity = true;
    [System.NonSerialized] private bool aerial;
    [System.NonSerialized] private Rigidbody rb;
    [System.NonSerialized] private CollisionManagement collisionManagement;

    public void Initialize(Rigidbody rb, CollisionManagement collisionManagement, bool aerial)
    {
        this.rb = rb;
        this.collisionManagement = collisionManagement;
        this.aerial = aerial;
        rb.useGravity = false;
        collisionManagement.CollisionDataCollected += GravityLoop;
    }

    public void Enable()
    {
    }

    public void Disable()
    {
        //collisionManagement.CollisionDataCollected -= GravityLoop;
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