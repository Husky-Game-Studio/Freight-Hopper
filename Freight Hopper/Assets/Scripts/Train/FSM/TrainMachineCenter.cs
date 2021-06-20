using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrainMachineCenter : FiniteStateMachineCenter
{
    private TrainStateTransitions transitionHandler;

    // States

    public FindNextPathState findNextPath;
    public FollowPathState followPath;
    public RefindPathState refindPath;
    public WaitingState waiting;
    public WanderState wander;

    // Independent Data
    [SerializeField] private List<Rigidbody> carriers;

    [SerializeField] private Optional<float> startWaitTime;
    [SerializeField] private Optional<float> startWhenDistanceFromPlayer;
    public Optional<float> StartWaitTime => startWaitTime;
    public Optional<float> StartWhenDistanceFromPlayer => startWhenDistanceFromPlayer;

    [SerializeField]
    private List<RoadCreator> pathObjects;

    private List<Vector3> spawnLocations;
    private List<Quaternion> spawnRotations;

    private int currentPath = 0;
    [SerializeField] private float targetVelocity;
    public float TargetVelocity => targetVelocity;
    [SerializeField] private float followDistance;
    public float FollowDistance => followDistance;
    [SerializeField] private Vector3 followOffset;
    public Vector3 FollowOffset => followOffset;

    [SerializeField] private Vector3 forceBounds;
    public Vector3 ForceBounds => forceBounds;
    [SerializeField] private Vector3 torqueBounds;
    public Vector3 TorqueBounds => torqueBounds;

    [HideInInspector] public PhysicsManager physicsManager;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public HoverController hoverController;

    public Vector3 TargetPos(float t)
    {
        return GetCurrentPathObject().GetPositionOnPath(t) + FollowOffset;
    }

    public BezierPath GetCurrentPath()
    {
        return pathObjects[currentPath].pathCreator.path;
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
        physicsManager = GetComponent<PhysicsManager>();
        rb = physicsManager.rb;
        hoverController = GetComponentInChildren<HoverController>();

        transitionHandler = new TrainStateTransitions(this);

        // Default
        List<Func<BasicState>> defaultTransitionsList = new List<Func<BasicState>>();
        defaultTransitionsList.Add(transitionHandler.CheckStartState);
        defaultState = new DefaultState(this, defaultTransitionsList);

        // Find Next Path
        List<Func<BasicState>> findNextPathTransitionsList = new List<Func<BasicState>>();
        findNextPath = new FindNextPathState(this, findNextPathTransitionsList);

        // Follow Path
        List<Func<BasicState>> followPathTransitionsList = new List<Func<BasicState>>();
        followPath = new FollowPathState(this, followPathTransitionsList);

        // Refind Path
        List<Func<BasicState>> refindPathTransitionsList = new List<Func<BasicState>>();
        refindPath = new RefindPathState(this, refindPathTransitionsList);

        // Waiting
        List<Func<BasicState>> waitingTransitionsList = new List<Func<BasicState>>();
        waitingTransitionsList.Add(transitionHandler.CheckFindNextPath);
        waiting = new WaitingState(this, waitingTransitionsList);

        // Wander
        List<Func<BasicState>> wanderTransitionsList = new List<Func<BasicState>>();
        wander = new WanderState(this, wanderTransitionsList);
    }

    public override void RestartFSM()
    {
        base.RestartFSM();
        this.transform.position = spawnLocations[0];
        this.transform.rotation = spawnRotations[0];
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        for (int i = 0; i < carriers.Count; i++)
        {
            carriers[i].position = spawnLocations[i + 1];
            carriers[i].rotation = spawnRotations[i + 1];
            carriers[i].velocity = Vector3.zero;
            carriers[i].angularVelocity = Vector3.zero;
        }
    }

    public void OnEnable()
    {
        spawnLocations = new List<Vector3>();
        spawnRotations = new List<Quaternion>();
        spawnLocations.Add(this.transform.position);
        spawnRotations.Add(this.transform.rotation);
        for (int i = 0; i < carriers.Count; i++)
        {
            spawnLocations.Add(carriers[i].position);
            spawnRotations.Add(carriers[i].rotation);
        }
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