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

    // Independent Data
    [SerializeField] private float derailDistance;
    [SerializeField] private float railSnappingDistance;
    [SerializeField] private Optional<float> startWaitTime;
    [SerializeField] private Optional<float> startWhenDistanceFromPlayer;
    [SerializeField] private List<RoadCreator> pathObjects;
    [SerializeField] private float targetVelocity;
    [SerializeField] private float followDistance;
    [SerializeField] private Vector3 followOffset;
    [SerializeField] private Vector3 forceBounds;
    [SerializeField] private Vector3 torqueBounds;

    [SerializeField, ReadOnly] private int currentPath = -1;

    // Accessors
    public bool OnFinalPath => currentPath == pathObjects.Count - 1;
    public float DerailDistance => derailDistance;
    public float RailSnappingDistance => railSnappingDistance;
    public float TargetVelocity => targetVelocity;
    public float FollowDistance => followDistance;
    public Vector3 FollowOffset => followOffset;
    public Vector3 ForceBounds => forceBounds;
    public Vector3 TorqueBounds => torqueBounds;
    public Optional<float> StartWaitTime => startWaitTime;
    public Optional<float> StartWhenDistanceFromPlayer => startWhenDistanceFromPlayer;

    // Dependecies
    [HideInInspector] public PhysicsManager[] physicsManagers;
    [HideInInspector] public Rigidbody[] rb;
    [HideInInspector] public HoverController hoverController;
    private TrainStateTransitions transitionHandler;

    public Vector3 TargetPos(float t)
    {
        return GetCurrentPathObject().GetPositionOnPath(t) + FollowOffset;
    }

    public BezierPath GetCurrentPath()
    {
        return pathObjects[currentPath].pathCreator.path;
    }

    public Vector3 GetStartOfCurrentPath()
    {
        return GetCurrentPathObject().GetPositionOnPath(0) + FollowOffset;
    }

    public RoadCreator GetCurrentPathObject()
    {
        return pathObjects[currentPath];
    }

    public void ChangePath()
    {
        currentPath++;
    }

    private void Awake()
    {
        physicsManagers = GetComponentsInChildren<PhysicsManager>();
        rb = new Rigidbody[physicsManagers.Length];
        for (int i = 0; i < physicsManagers.Length; i++)
        {
            rb[i] = physicsManagers[i].rb;
        }

        hoverController = rb[0].GetComponentInChildren<HoverController>();

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

    public void Follow(Vector3 targetPosition)
    {
        if (!hoverController.EnginesFiring)
        {
            return;
        }

        Vector3 target = targetPosition - rb[0].position;
        Quaternion rot = rb[0].transform.rotation;
        Quaternion rotInv = Quaternion.Inverse(rot);
        Vector3 currentVelDir = (rb[0].velocity.magnitude != 0) ? rb[0].velocity.normalized : Vector3.forward;
        Quaternion rotVel = Quaternion.LookRotation(currentVelDir);
        Quaternion rotVelInv = Quaternion.Inverse(rotVel);
        Debug.DrawLine(rb[0].position, rb[0].position + currentVelDir * 20.0f, Color.magenta);

        //Current
        Vector3 currentVel = rb[0].velocity;
        Vector3 currentAngVel = rb[0].angularVelocity;

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
        acc = rot * (rotInv * acc).ClampComponents(new Vector3(-ForceBounds.x, 0, 0), ForceBounds);
        angAcc = rot * (rotInv * angAcc).ClampComponents(-TorqueBounds, TorqueBounds);

        //Forces
        rb[0].AddForce(acc, ForceMode.Acceleration);
        rb[0].AddTorque(angAcc, ForceMode.Acceleration);

        //Rolling Correction
        float z = rb[0].transform.eulerAngles.z;
        z -= (z > 180) ? 360 : 0;
        rb[0].AddRelativeTorque(Vector3.forward * -0.05f * z / Time.fixedDeltaTime, ForceMode.Acceleration);
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