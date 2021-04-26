using UnityEngine;

public class GroundPoundBehavior : AbilityBehavior
{
    //[ReadOnly, SerializeField] private bool groundPoundPossible = true;
    //[ReadOnly, SerializeField] private bool groundPounding = false;
    [ReadOnly, SerializeField] private Var<float> increasingForce = new Var<float>(1, 1);

    [SerializeField] private float deltaIncreaseForce = 0.01f;
    [SerializeField] private float initialBurstForce = 20;
    [SerializeField] private float downwardsForce = 10;
    [SerializeField] private float slopeDownForce = 500;

    public override void EntryAction()
    {
        if (playerRb.velocity.y > 0) // this line
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
        }
        playerRb.AddForce(-playerCM.ValidUpAxis * initialBurstForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        // Checks if ground pound is possible
        /*if (!UserInput.Input.GroundPoundTriggered()) // if the user is not holding the input, stop ground pounding ~~ let go of input
        {
            groundPoundPossible = true;
            groundPounding = false;
        }*/

        //if (groundPounding && playerCollision.ContactNormal.current != upAxis) // if holding input and surface is not flat. the player is either on the ground or not on the ground ??
        //{
        Vector3 upAxis = playerCM.ValidUpAxis;
        Vector3 direction = -upAxis;
        if (playerCM.IsGrounded.current) // if on ground, change the direction of groundpound
        {
            Vector3 acrossSlope = Vector3.Cross(upAxis, playerCM.ContactNormal.current);
            Vector3 downSlope = Vector3.Cross(acrossSlope, playerCM.ContactNormal.current);
            direction = downSlope;
            direction *= slopeDownForce;
        }
        else // else maintain the regular ground pound direction
        {
            direction *= downwardsForce;
        }

        playerRb.AddForce(direction * increasingForce.current, ForceMode.Acceleration);
        increasingForce.current += deltaIncreaseForce * Time.fixedDeltaTime; // the longer you are holding the input, the greater the groundpound force
        //}
    }

    public override void ExitAction()
    {
        base.ExitAction();
        increasingForce.RevertCurrent();
    }
}