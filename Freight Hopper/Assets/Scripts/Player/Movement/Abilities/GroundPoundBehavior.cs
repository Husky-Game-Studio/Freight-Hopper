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
    [SerializeField] private ParticleSystem particles;
    [Space]
    [SerializeField]
    private MovementBehavior movementBehavior;
    [SerializeField] private float lowSpeedMultiplier = 2;
    [SerializeField, Tooltip("the horizontal speed you have to be below to apply low speed multiplier")]
    private float horizontalSpeedLimit = 90f;
    

    private bool active;

    public float FrictionReduction => groundFrictionReductionPercent;
    public bool Active => active;

    public bool FlatSurface =>
         collisionManager.IsGrounded.current && Vector3.Angle(collisionManager.ValidUpAxis, collisionManager.ContactNormal.current) < angleToBeConsideredFlat;
    private CollisionManagement collisionManager;
    private Friction friction;

    public override void Initialize()
    {
        base.Initialize();
        collisionManager = Player.Instance.modules.collisionManagement;
        friction = Player.Instance.modules.friction;
    }

    public void EntryAction()
    {
        GroundPoundInitialBurst();
        soundManager.Play("GroundPoundBurst");
        Vector3 upAxis = collisionManager.ValidUpAxis;
        if (Vector3.Dot(Vector3.Project(rb.velocity, upAxis), rb.transform.up) > 0)
        {
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, upAxis);
        }
        active = true;
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

    public void Action()
    {
        friction.ReduceFriction(FrictionReduction);
        soundManager.Play("GroundPoundTick");
        Vector3 upAxis = collisionManager.ValidUpAxis;
        
        Vector3 direction = -upAxis;
        //Debug.Log("ground pounding");
        if (collisionManager.IsGrounded.current)
        {
            
            Vector3 acrossSlope = Vector3.Cross(upAxis, collisionManager.ContactNormal.current);
            Vector3 downSlope = Vector3.Cross(acrossSlope, collisionManager.ContactNormal.current);
            direction = downSlope;

            if (!FlatSurface)
            {
                soundManager.Play("WallSkid");
            }
            
            if (!collisionManager.IsGrounded.old)
            {
                particles.Play();
                if (!this.FlatSurface){
                    Vector3 oldDownForce = Vector3.Project(collisionManager.Velocity.old, upAxis);
                    rb.AddForce(direction * oldDownForce.magnitude, ForceMode.VelocityChange);
                }
                
            }
            direction *= slopeDownForce;
            if (movementBehavior.HorizontalSpeed < horizontalSpeedLimit)
            {
                direction *= lowSpeedMultiplier;
            }
        }
        else
        {
            direction *= downwardsForce;
            StartCoroutine(soundManager.Stop("WallSkid"));
        }

        

        rb.AddForce(direction * increasingForce.value, ForceMode.Acceleration);
        increasingForce.value += deltaIncreaseForce * Time.fixedDeltaTime;
    }

    public void ExitAction()
    {
        if (!active)
        {
            return;
        }
        PreventConsumptionCheck();

        soundManager.Play("GroundPoundExit");
        StartCoroutine(soundManager.Stop("WallSkid"));
        increasingForce.Reset();
        active = false;
    }
}