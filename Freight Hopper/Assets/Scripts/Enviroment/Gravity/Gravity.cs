using UnityEngine;

[System.Serializable]
public class Gravity : MonoBehaviour
{
    [SerializeField] private bool useGravity = true;
    [SerializeField] private bool aerial;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CollisionManagement collisionManagement;
    private void Awake()
    {
        rb.useGravity = false;
        if(collisionManagement != null)
        {
            collisionManagement.CollisionDataCollected += GravityLoop;
        }
    }
    public void Initialize(Rigidbody rb, CollisionManagement collisionManagement, bool aerial)
    {
        this.rb = rb;
        this.collisionManagement = collisionManagement;
        this.aerial = aerial;
        rb.useGravity = false;
        collisionManagement.CollisionDataCollected += GravityLoop;
    }
    private void FixedUpdate()
    {
        if(collisionManagement == null)
        {
            GravityLoop();
        }
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
        if (!useGravity || (collisionManagement != null && collisionManagement.IsGrounded.current && !aerial))
        {
            return;
        }
        rb.AddForce(CustomGravity.GetGravity(rb.position), ForceMode.Acceleration);
    }
}