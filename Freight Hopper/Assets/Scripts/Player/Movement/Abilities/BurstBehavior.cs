using UnityEngine;

public class BurstBehavior : AbilityBehavior
{
    //private bool burstPossible = true;
    [SerializeField] private float burstForce;

    [Range(-90, 90)] [SerializeField] private float angle; // with 0 being level with the ground

    // [SerializeField] private Timer delay = new Timer(0.5f);

    //private bool dash = false;

    private void OnDrawGizmosSelected()
    {
        float radians = angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians));
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(direction));
    }

    /*    private void Update()
        {
            //delay.CountDown();
            //if (UserInput.Input.Dash() && !delay.TimerActive() && burstPossible)
            //{
            *//*            dash = true;
                        burstPossible = false;
                        delay.ResetTimer();
                    //}

                    if (playerCollision.IsGrounded.current)
                    {
                        burstPossible = true;
                    }*//*
        }*/

    public override void EntryAction()
    {
        float radians = angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians));
        playerRb.AddForce(transform.TransformDirection(direction) * burstForce, ForceMode.VelocityChange);
        //dash = false;
        //burstPossible = false;
    }

    public override void Action()
    {
    }
}