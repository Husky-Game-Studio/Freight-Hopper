using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrainMachineCenter : FiniteStateMachineCenter
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
    [SerializeField] public bool derailToWait = false;
    [SerializeField] private List<RoadCreator> pathObjects;
    [SerializeField] private float targetVelocity;
    [SerializeField] private Vector3 forceBounds;
    [SerializeField] private Vector3 torqueBounds;

    [SerializeField, ReadOnly] private int currentPath = -1;

    // Accessors
    public bool OnFinalPath => currentPath == pathObjects.Count - 1;
    public float TargetVelocity => targetVelocity;
    public Vector3 ForceBounds => forceBounds;
    public Vector3 TorqueBounds => torqueBounds;
    public Optional<float> StartWaitTime => startWaitTime;
    public Optional<float> StartWhenDistanceFromPlayer => startWhenDistanceFromPlayer;

    // Dependecies
    [HideInInspector] public PhysicsManager[] physicsManagers;
    [HideInInspector] public Rigidbody[] cartRigidbodies;
    [HideInInspector] public HoverController hoverController;
    [HideInInspector] public TrainRailLinker currentRailLinker;
    private TrainStateTransitions transitionHandler;

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
            currentRailLinker = GetCurrentPathObject().GetComponent<TrainRailLinker>();
        }
    }

    private void Awake()
    {
        physicsManagers = GetComponentsInChildren<PhysicsManager>();
        cartRigidbodies = new Rigidbody[physicsManagers.Length];
        for (int i = 0; i < physicsManagers.Length; i++)
        {
            cartRigidbodies[i] = physicsManagers[i].rb;
        }

        hoverController = cartRigidbodies[0].GetComponentInChildren<HoverController>();

        transitionHandler = new TrainStateTransitions(this);

        // Default
        List<Func<BasicState>> defaultTransitionsList = new List<Func<BasicState>>();
        defaultTransitionsList.Add(transitionHandler.CheckStartState);
        defaultState = new DefaultState(this, defaultTransitionsList);

        // Find Next Path
        List<Func<BasicState>> findNextPathTransitionsList = new List<Func<BasicState>>();
        findNextPathTransitionsList.Add(transitionHandler.CheckFollowPath);
        findNextPath = new FindNextPathState(this, findNextPathTransitionsList);

        // Follow Path
        List<Func<BasicState>> followPathTransitionsList = new List<Func<BasicState>>();
        followPathTransitionsList.Add(transitionHandler.CheckFindNextPath);
        followPathTransitionsList.Add(transitionHandler.CheckWander);
        followPath = new FollowPathState(this, followPathTransitionsList);

        // Waiting
        List<Func<BasicState>> waitingTransitionsList = new List<Func<BasicState>>();
        waitingTransitionsList.Add(transitionHandler.CheckFindNextPath);
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

        Vector3 target = targetPosition - cartRigidbodies[0].position;
        Quaternion rot = cartRigidbodies[0].transform.rotation;
        Quaternion rotInv = Quaternion.Inverse(rot);
        Vector3 currentVelDir = (cartRigidbodies[0].velocity.magnitude != 0) ? cartRigidbodies[0].velocity.normalized : Vector3.forward;
        Quaternion rotVel = Quaternion.LookRotation(currentVelDir);
        Quaternion rotVelInv = Quaternion.Inverse(rotVel);
        Debug.DrawLine(cartRigidbodies[0].position, cartRigidbodies[0].position + currentVelDir * 20.0f, Color.magenta);

        //Current
        Vector3 currentVel = cartRigidbodies[0].velocity;
        Vector3 currentAngVel = cartRigidbodies[0].angularVelocity;

        //Target (Rotate, Move Forward)
        Vector3 targetVel = rot * Vector3.forward * TargetVelocity;
        Vector3 targetAngVel = currentVel.magnitude * (rot * TargetAngVel(rotInv * target)); //Rotate based on rotation heading

        //Target Change
        Vector3 deltaVel = targetVel - currentVel;
        Vector3 deltaAngVel = targetAngVel - currentAngVel;

        //Target Acceleration
        Vector3 acc = deltaVel / Time.fixedDeltaTime;
        Vector3 angAcc = deltaAngVel / Time.fixedDeltaTime;

        //Limit Change
        acc = rot * (rotInv * acc).ClampComponents(-ForceBounds, ForceBounds);
        angAcc = rot * (rotInv * angAcc).ClampComponents(-TorqueBounds, TorqueBounds);

        //Forces
        cartRigidbodies[0].AddForce(acc, ForceMode.Acceleration);
        cartRigidbodies[0].AddTorque(angAcc, ForceMode.Acceleration);

        //Rolling Correction
        float z = cartRigidbodies[0].transform.eulerAngles.z;
        z -= (z > 180) ? 360 : 0;
        cartRigidbodies[0].AddRelativeTorque(Vector3.forward * -0.05f * z / Time.fixedDeltaTime, ForceMode.Acceleration);
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
        currentState.ExitState();
        LevelController.PlayerRespawned -= RestartFSM;
    }

    private void FixedUpdate()
    {
        UpdateLoop();
    }
}