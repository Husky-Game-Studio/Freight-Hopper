using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class TrainMachineCenter : FiniteStateMachineCenter
{
    // States
    public FindNextPathState findNextPath;
    public FollowPathState followPath;
    public WaitingState waiting;
    public WanderState wander;
    [Space]
    // Independent Data
    [SerializeField] private Optional<float> startWaitTime;
    [SerializeField] private Optional<float> startWhenDistanceFromPlayer;
    [SerializeField] private bool derailToWait = false;
    [SerializeField] private bool loop = false;
    [SerializeField] private List<RoadCreator> pathObjects;
    [SerializeField] private float targetVelocity;
    [SerializeField] private Vector3 forceBounds;
    [SerializeField] private Vector3 torqueBounds;

    [SerializeField, ReadOnly] private int currentPath = -1;

    // Accessors
    public bool OnFinalPath => currentPath == pathObjects.Count - 1 && !loop;
    public bool DerailToWait => derailToWait;
    public bool Loop => loop;
    public float TargetVelocity => targetVelocity;
    public Vector3 ForceBounds => forceBounds;
    public Vector3 TorqueBounds => torqueBounds;
    public Optional<float> StartWaitTime => startWaitTime;
    public Optional<float> StartWhenDistanceFromPlayer => startWhenDistanceFromPlayer;

    // Dependecies
    [HideInInspector, NonSerialized] public LinkedList<Cart> carts = new LinkedList<Cart>();
    [HideInInspector] public HoverController hoverController;
    [HideInInspector] public TrainRailLinker currentRailLinker;
    private TrainStateTransitions transitionHandler;

    private void OnDrawGizmosSelected()
    {
        if (startWhenDistanceFromPlayer.Enabled)
        {
            Vector3 position = this.transform.position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, startWhenDistanceFromPlayer.value);
        }
    }

    // Returns current BezierPath
    public BezierPath GetCurrentPath()
    {
        if (currentPath >= pathObjects.Count)
        {
            return null;
        }
        return pathObjects[currentPath].pathCreator.path;
    }

    // Returns the start of the current path the train is trying to reach
    public Vector3 GetStartOfCurrentPath()
    {
        return GetCurrentPathObject().GetPositionOnPath(0) + currentRailLinker.Offset;
    }

    public float GetClosestTValueOnCurrentPath()
    {
        return GetCurrentPathObject().FindClosestT(carts.First.Value.rb.position);
    }

    public Vector3 GetClosestPointOnCurrentPath()
    {
        return GetCurrentPathObject().GetPositionOnPath(GetClosestTValueOnCurrentPath()) + currentRailLinker.Offset;
    }

    // Returns the RoadCreator object which contains the current path. Good for getting the object the path is likely on
    public RoadCreator GetCurrentPathObject()
    {
        if (currentPath >= pathObjects.Count)
        {
            return null;
        }
        return pathObjects[currentPath];
    }

    // Updates the current path to be the next one if it does exist
    public void ChangePath()
    {
        currentPath++;
        if (currentPath < pathObjects.Count)
        {
            currentRailLinker = GetCurrentPathObject().pathCreator.GetComponent<TrainRailLinker>();
        }
        else if (loop && pathObjects.Count > 0)
        {
            currentPath = 0;
        }
    }

    public void RemoveCartsUntilIndex(int index)
    {
        if (index == 0)
        {
            currentState?.ExitState();
            previousState = null;
            currentState = null;
            Destroy(this, carts.First.Value.destructable.ExplosionTime);
        }
        for (int i = carts.Count; i > index; i--)
        {
            Cart cart = carts.Last.Value;
            cart.destructable.RigidbodyDestroyed -= cart.DestroyCartFunc;
            cart.DestoryCart -= RemoveCartsUntilIndex;
            carts.RemoveLast();
        }
    }

    private void Awake()
    {
        PhysicsManager[] physics = GetComponentsInChildren<PhysicsManager>();

        for (int i = 0; i < physics.Length; i++)
        {
            Cart cart = new Cart(physics[i]);
            cart.destructable.RigidbodyDestroyed += cart.DestroyCartFunc;
            cart.DestoryCart += RemoveCartsUntilIndex;
            carts.AddLast(cart);
        }

        hoverController = carts.First.Value.rb.GetComponentInChildren<HoverController>();

        transitionHandler = new TrainStateTransitions(this);

        // Default
        List<Func<BasicState>> defaultTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckStartState
        };
        defaultState = new DefaultState(this, defaultTransitionsList);

        // Find Next Path
        List<Func<BasicState>> findNextPathTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckFollowPath
        };
        findNextPath = new FindNextPathState(this, findNextPathTransitionsList);

        // Follow Path
        List<Func<BasicState>> followPathTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckFindNextPath,
            transitionHandler.CheckWander
        };
        followPath = new FollowPathState(this, followPathTransitionsList);

        // Waiting
        List<Func<BasicState>> waitingTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckFindNextPath
        };
        waiting = new WaitingState(this, waitingTransitionsList);

        // Wander
        List<Func<BasicState>> wanderTransitionsList = new List<Func<BasicState>>();
        wander = new WanderState(this, wanderTransitionsList);
    }

    // Given a position, the train will try to rotate and move towards that position
    public void Follow(Vector3 targetPosition)
    {
        if (!hoverController.EnginesFiring)
        {
            return;
        }

        Vector3 target = targetPosition - carts.First.Value.rb.position;
        Quaternion rot = carts.First.Value.rb.transform.rotation;
        Quaternion rotInv = Quaternion.Inverse(rot);
        Vector3 currentVelDir = (carts.First.Value.rb.velocity.magnitude != 0) ? carts.First.Value.rb.velocity.normalized : Vector3.forward;
        Debug.DrawLine(carts.First.Value.rb.position, carts.First.Value.rb.position + (currentVelDir * 20.0f), Color.magenta);

        //Current
        Vector3 currentVel = carts.First.Value.rb.velocity;
        Vector3 currentAngVel = carts.First.Value.rb.angularVelocity;

        //Target (Rotate, Move Forward)
        Vector3 targetVel = rot * Vector3.forward * this.TargetVelocity;
        Vector3 targetAngVel = currentVel.magnitude * (rot * TargetAngVel(rotInv * target)); //Rotate based on rotation heading

        //Target Change
        Vector3 deltaVel = targetVel - currentVel;
        Vector3 deltaAngVel = targetAngVel - currentAngVel;

        //Target Acceleration
        Vector3 acc = deltaVel / Time.fixedDeltaTime;
        Vector3 angAcc = deltaAngVel / Time.fixedDeltaTime;

        //Limit Change
        acc = rot * (rotInv * acc).ClampComponents(-this.ForceBounds, this.ForceBounds);
        angAcc = rot * (rotInv * angAcc).ClampComponents(-this.TorqueBounds, this.TorqueBounds);

        //Forces
        carts.First.Value.rb.AddForce(acc, ForceMode.Acceleration);
        carts.First.Value.rb.AddTorque(angAcc, ForceMode.Acceleration);

        //Rolling Correction
        foreach (Cart cart in carts)
        {
            Vector3 gravityForward = Vector3.Cross(CustomGravity.GetUpAxis(cart.rb.position), cart.rb.transform.right);

            Vector3 gravityUp = Vector3.ProjectOnPlane(CustomGravity.GetUpAxis(cart.rb.position), gravityForward);
            Vector3 cartUp = Vector3.ProjectOnPlane(cart.rb.transform.up, gravityForward);

            float angleWrong = Vector3.SignedAngle(gravityUp, cartUp, cart.rb.transform.forward);

            cart.rb.AddRelativeTorque(Vector3.forward * -0.05f * angleWrong, ForceMode.VelocityChange);
        }
    }

    private Vector3 TargetAngVel(Vector3 target)
    {
        return 2 * new Vector3(-target.y, target.x, 0) / target.sqrMagnitude;
    }

    public override void PerformStateIndependentBehaviors()
    {
    }

    public override void RestartFSM()
    {
        base.RestartFSM();
    }

    public void OnEnable()
    {
        RestartFSM();
        LevelController.PlayerRespawned += RestartFSM;
    }

    public void OnDisable()
    {
        currentState?.ExitState();
        LevelController.PlayerRespawned -= RestartFSM;
    }

    private void FixedUpdate()
    {
        UpdateLoop();
    }
}