using UnityEngine;

public class GroundPoundBehavior : AbilityBehavior
{
    [ReadOnly, SerializeField] private Current<float> increasingForce = new Current<float>(1);

    [SerializeField] private float deltaIncreaseForce = 0.01f;
    [SerializeField] private float angleToBeConsideredFlat;
    [SerializeField] private float initialBurstVelocity = 20;
    [SerializeField] private float downwardsForce = 10;
    [SerializeField] private float slopeDownForce = 500;
    [SerializeField] private float groundFrictionReductionPercent = 0.95f;

    [Header("Camera Shake Settings")]
    [SerializeField] private float slamSpeedToReachBase = 125f;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float baseShakeStrength = 5;
    //[SerializeField] private float cameraSwayTime = 0.1f;
    //[SerializeField] private float cameraSwayMag = 1;
    public float FrictionReduction => groundFrictionReductionPercent;
    public bool FlatSurface =>
         collisionManager.IsGrounded.current
            && Vector3.Angle(collisionManager.ValidUpAxis, collisionManager.ContactNormal.current) < angleToBeConsideredFlat;
    private CollisionManagement collisionManager;

    public override void Initialize()
    {
        base.Initialize();
        collisionManager = Player.Instance.modules.collisionManagement;
    }

    public override void EntryAction()
    {
        soundManager.Play("GroundPoundBurst");
        Vector3 upAxis = collisionManager.ValidUpAxis;
        if (Vector3.Dot(Vector3.Project(rb.velocity, upAxis), rb.transform.up) > 0)
        {
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, upAxis);
        }
    }

    public void GroundPoundInitialBurst()
    {   
        bool underSpeedLimit = initialBurstVelocity > -collisionManager.Velocity.old.y;
        if (!underSpeedLimit)
        {
            return;
        }
            
        Vector3 upAxis = collisionManager.ValidUpAxis;
        rb.AddForce(-upAxis * initialBurstVelocity, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        soundManager.Play("GroundPoundTick");
        Vector3 upAxis = collisionManager.ValidUpAxis;
        Vector3 direction = -upAxis;
        if (collisionManager.IsGrounded.current)
        {
            Vector3 acrossSlope = Vector3.Cross(upAxis, collisionManager.ContactNormal.current);
            Vector3 downSlope = Vector3.Cross(acrossSlope, collisionManager.ContactNormal.current);
            direction = downSlope;
            if (!collisionManager.IsGrounded.old &&
                !this.FlatSurface)
            {
                Vector3 oldDownForce = Vector3.Project(collisionManager.Velocity.old, upAxis);
                rb.AddForce(direction * oldDownForce.magnitude, ForceMode.VelocityChange);
            }
            direction *= slopeDownForce;
        }
        else
        {
            direction *= downwardsForce;
        }

        rb.AddForce(direction * increasingForce.value, ForceMode.Acceleration);
        increasingForce.value += deltaIncreaseForce * Time.fixedDeltaTime;
    }

    public override void ExitAction()
    {
        base.ExitAction();
        soundManager.Play("GroundPoundExit");
        increasingForce.Reset();
        if (FlatSurface)
        {
            float speed = Vector3.Project(collisionManager.Velocity.old, rb.transform.up).magnitude;
            float slamShakeModified = speed / slamSpeedToReachBase;
            //Player.Instance.modules.cameraShake.StartCameraShake(shakeDuration, new CameraShake.TraumaSettings(baseShakeStrength * slamShakeModified));
            //Debug.Log("performned a ground pound at a speed of " + speed + " actual old: " + collisionManager.Velocity.old.magnitude);
            //Player.Instance.modules.cameraShake.StartCameraSway(cameraSwayTime * slamShakeModified, -collisionManager.ContactNormal.old, cameraSwayMag * slamShakeModified);
        }
    }
}