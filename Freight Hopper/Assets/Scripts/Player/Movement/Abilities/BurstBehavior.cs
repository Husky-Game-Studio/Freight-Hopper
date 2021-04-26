using UnityEngine;

public class BurstBehavior : AbilityBehavior
{
    [SerializeField] private float burstForce;

    [Range(-90, 90)] [SerializeField] private float angle; // with 0 being level with the ground

    // [SerializeField] private Timer delay = new Timer(0.5f);

    private void OnDrawGizmosSelected()
    {
        float radians = angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians));
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(direction));
    }

    public override void EntryAction()
    {
    }

    public override void Action()
    {
        float radians = angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians));
        playerRb.AddForce(transform.TransformDirection(direction) * burstForce, ForceMode.VelocityChange);
    }
}