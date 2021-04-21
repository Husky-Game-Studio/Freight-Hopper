using UnityEngine;

[System.Serializable]
public class GroundPoundBehavior : MonoBehaviour
{
    [ReadOnly, SerializeField] private bool groundPoundPossible = true;
    [ReadOnly, SerializeField] private bool groundPounding = false;
    [ReadOnly, SerializeField] private Var<float> increasingForce = new Var<float>(1, 1);
    [SerializeField] private float deltaIncreaseForce = 0.01f;
    [SerializeField] private float initialBurstForce = 20;
    [SerializeField] private float downwardsForce = 10;
    [SerializeField] private float slopeDownForce = 500;

    private CollisionManagement playerCollision;
    private Rigidbody rb;

    public void Initalize(Rigidbody rb, CollisionManagement playerCollision)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
    }

    private void OnEnable()
    {
        UserInput.Input.GroundPoundInput += TryGroundPound;
        playerCollision.CollisionDataCollected += GroundPounding;
    }

    private void OnDisable()
    {
        UserInput.Input.GroundPoundInput -= TryGroundPound;
        playerCollision.CollisionDataCollected -= GroundPounding;
    }

    // I need to move all of this to the PFSM check
    public void TryGroundPound()
    {
        if (groundPoundPossible)
        {
            if (rb.velocity.y > 0) // this line
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
            rb.AddForce(-CustomGravity.GetUpAxis(rb.position) * initialBurstForce, ForceMode.VelocityChange); // to this line, is the initial state
            groundPounding = true; // we are currently ground pounding
            groundPoundPossible = false;
        }
    }

    public void GroundPounding()
    {
        // Checks if ground pound is possible
        if (!UserInput.Input.GroundPoundTriggered()) // if the user is not holding the input, stop ground pounding ~~ let go of input
        {
            groundPoundPossible = true;
            groundPounding = false;
        }
        Vector3 upAxis = CustomGravity.GetUpAxis(rb.position);
        if (groundPounding && playerCollision.ContactNormal.current != upAxis) // if holding input and surface is not flat. the player is either on the ground or not on the ground ??
        {
            Vector3 direction = -upAxis;
            if (playerCollision.IsGrounded.current) // if on ground, change the direction of groundpound
            {
                Vector3 acrossSlope = Vector3.Cross(upAxis, playerCollision.ContactNormal.current);
                Vector3 downSlope = Vector3.Cross(acrossSlope, playerCollision.ContactNormal.current);
                direction = downSlope;
                direction *= slopeDownForce;
            }
            else // else maintain the regular ground pound direction
            {
                direction *= downwardsForce;
            }

            rb.AddForce(direction * increasingForce.current, ForceMode.Acceleration);
            increasingForce.current += deltaIncreaseForce; // the longer you are holding the input, the greater the groundpound force
        }
        else // on exit
        {
            groundPounding = false; // can remove ??
            increasingForce.RevertCurrent();
        }
    }
}