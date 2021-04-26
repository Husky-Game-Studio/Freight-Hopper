using UnityEngine;

public class FullStopBehavior : AbilityBehavior
{
    public override void Action()
    {
        playerRb.velocity = Vector3.zero;
    }

    public override void EntryAction()
    {
    }
}