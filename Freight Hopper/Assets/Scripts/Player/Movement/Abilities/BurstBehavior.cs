using UnityEngine;

public class BurstBehavior : AbilityBehavior
{
    [SerializeField] private float airBurstForce;

    // with 0 being level with the ground
    [Range(-90, 90)] [SerializeField] private float burstAngle;

    [SerializeField] private float groundBurstForce;
    [Range(-90, 90)] [SerializeField] private float groundBurstAngle;

    [SerializeField] private float wallBurstForce;
    [Range(-90, 90)] [SerializeField] private float wallBurstAngle;
    private Transform cameraTransform;

    private void OnDrawGizmosSelected()
    {
        float radians = burstAngle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians));
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(this.transform.position, this.transform.TransformDirection(direction));
    }

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    public override void EntryAction()
    {
    }

    public override void Action()
    {
        AirBurst();
    }

    public void GroundBurst()
    {
        float radians = groundBurstAngle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians));
        playerRb.AddForce(this.cameraTransform.TransformDirection(direction) * groundBurstForce, ForceMode.VelocityChange);
    }

    public void WallBurst()
    {
        float radians = wallBurstAngle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians));
        playerRb.AddForce(this.cameraTransform.TransformDirection(direction) * wallBurstForce, ForceMode.VelocityChange);
    }

    public void AirBurst()
    {
        float radians = burstAngle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians));
        playerRb.AddForce(this.cameraTransform.TransformDirection(direction) * airBurstForce, ForceMode.VelocityChange);
    }
}