using UnityEngine;

public class FullStopBehavior : AbilityBehavior
{
    //[SerializeField, ReadOnly] private bool fullStopPossible = true;
    //[SerializeField] public Timer cooldown = new Timer(2);

    /*private void OnEnable()
    {
        UserInput.Input.FullStopInput += TryFullStop;
        playerCollision.Landed += Landed;
        playerCollision.CollisionDataCollected += cooldown.CountDownFixed;
    }

    private void OnDisable()
    {
        UserInput.Input.FullStopInput -= TryFullStop;
        playerCollision.Landed -= Landed;
        playerCollision.CollisionDataCollected -= cooldown.CountDownFixed;
    }*/

    /*    public void Landed()
        {
            fullStopPossible = true;
        }*/

    /*  public void TryFullStop()
      {
          // Stops the player
          if (!cooldown.TimerActive() && fullStopPossible)
          {
          }
      }*/

    public override void EntryAction()
    {
    }

    public override void Action()
    {
        //cooldown.ResetTimer();
        playerRb.velocity = Vector3.zero;
        //fullStopPossible = false;
    }
}