using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class GroundPoundBehavior : AbilityBehavior
{
    [ReadOnly, SerializeField] private Current<float> increasingForce = new Current<float>(1);

    [SerializeField] private float deltaIncreaseForce = 0.01f;
    [SerializeField] private float forceMultiplier = 2f;
    [SerializeField] private float normalDetectDistance = 1.5f;
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

    
    public void ResetAllState()
    {
        active = false;
        increasingForce.Reset();
    }
    public override void Initialize()
    {
        base.Initialize();
        collisionManager = Player.Instance.modules.collisionManagement;
        friction = Player.Instance.modules.friction;
    }

    public void EntryAction() // need cooldown too bruh
    {
        GroundPoundInitialBurst();
        Vector3 upAxis = collisionManager.ValidUpAxis;
        if (Vector3.Dot(Vector3.Project(rb.velocity, upAxis), rb.transform.up) > 0)
        {
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, upAxis);
        }
        active = true;
    }

    public void GroundPoundInitialBurst()
    {
        Vector3 normal = Vector3.down;
        Rigidbody rbConnected = null;
        if (collisionManager.RaycastDown(normalDetectDistance, out RaycastHit hit))
        {
            normal = hit.normal;
            rbConnected = hit.rigidbody;
        }
        Vector3 direction = Vector3.down;
        float speedOnSurface = -collisionManager.Velocity.old.y;
        bool isDownward = normal == Vector3.down;
        if (!isDownward)
        {
            Vector3 connectedRbVelocity = rb == null ? Vector3.zero : rbConnected.velocity;
            speedOnSurface = (collisionManager.Velocity.old - connectedRbVelocity).ProjectOnContactPlane(normal).magnitude;
            Vector3 acrossSlope = Vector3.Cross(Vector3.up, normal);
            Vector3 downSlope = Vector3.Cross(acrossSlope, normal);
            direction = downSlope;
            Debug.Log("Not downwards");
        }

        bool underSpeedLimit = speedOnSurface < initialBurstVelocity;
        if (!underSpeedLimit)
        {
            return;
        }
        soundManager.Play("GroundPoundBurst");
        Debug.Log("FIRED");
        rb.AddForce(direction * initialBurstVelocity, ForceMode.VelocityChange);
    }

    public void Action()
    {
        friction.ReduceFriction(FrictionReduction);
        //soundManager.Play("GroundPoundTick");
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
                soundManager.Play("GroundPoundLand");
                if (!this.FlatSurface){
                    Vector3 oldDownForce = Vector3.Project(collisionManager.Velocity.old, upAxis);
                    rb.AddForce(direction * oldDownForce.magnitude, ForceMode.VelocityChange);
                }
                
            }
            direction *= slopeDownForce;
        }
        else
        {
            direction *= downwardsForce;
            soundManager.Stop("WallSkid");
        }

        

        rb.AddForce(direction * increasingForce.value * forceMultiplier, ForceMode.Acceleration);
        increasingForce.value += deltaIncreaseForce * Time.fixedDeltaTime;
    }

    public void ExitAction()
    {
        PreventConsumptionCheck();
        soundManager.Play("GroundPoundExit");
        soundManager.Stop("WallSkid");
        increasingForce.Reset();
        active = false;
    }
}