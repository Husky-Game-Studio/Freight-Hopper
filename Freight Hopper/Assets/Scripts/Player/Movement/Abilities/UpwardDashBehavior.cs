using UnityEngine;

public class UpwardDashBehavior : AbilityBehavior
{
    //private bool upwardDashPossible = true;
    [SerializeField, ReadOnly] private Vector3 storedVelocity;

    [SerializeField] private float initialUpwardsForce;
    [SerializeField] private float consistentForce;
    //[SerializeField] private Timer delay = new Timer(0.5f);
    //[SerializeField] private Timer dashDuration = new Timer(1);
    //private bool upwardDash = false;

    /*    private void Update()
        {
            delay.CountDown();
            dashDuration.CountDown();
            // Checks if button is pressed and timer is active and can dash within constrains
            if (UserInput.Input.UpwardDashTriggered() && !delay.TimerActive() && upwardDashPossible)
            {
                upwardDash = true;
                upwardDashPossible = false;
                delay.ResetTimer();
            }*/
    /*  // Checks if player is gorunded
      if (playerCollision.IsGrounded.current)
      {
          upwardDashPossible = true;
      }
      // adds force as long as timer is till active
      if (dashDuration.TimerActive())
      {
      }*/
    //}

    public override void EntryAction()
    {
        storedVelocity = playerRb.velocity;
        playerRb.velocity = Vector3.zero;
        playerRb.AddForce(playerCM.ValidUpAxis * initialUpwardsForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        //upwardDash = false;
        //dashDuration.ResetTimer();
        //upwardDashPossible = false;
        playerRb.AddForce(playerCM.ValidUpAxis * consistentForce);
        playerRb.velocity = new Vector3(0, playerRb.velocity.y, 0);
    }

    public override void ExitAction()
    {
        consumed = true;
        playerRb.velocity = storedVelocity;
        storedVelocity = Vector3.zero;
    }
}