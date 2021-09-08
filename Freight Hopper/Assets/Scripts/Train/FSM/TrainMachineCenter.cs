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
    [SerializeField] private Optional<OnTriggerEvent> startOnTriggerEnter;
    [SerializeField] private bool derailToWait = false;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool instantlyAccelerate = false;
    [SerializeField] private List<PathCreation.PathCreator> pathObjects;
    [SerializeField] private float targetVelocity;

    [SerializeField, ReadOnly] private int currentPath = -1;

    // Destroy Train After Derail Fields
    [SerializeField] private bool deleteOnDerail = false;

    [SerializeField] private float timeBeforeDelete = 10;
    public Timer timerToDelete;
    public bool trainDerailed;
    private bool startedAlready = false;

    // Accessors
    public bool OnFinalPath => currentPath == pathObjects.Count - 1 && !loop;
    public bool Starting
    {
        get
        {
            if (startedAlready)
            {
                return false;
            }
            if (currentPath == 0)
            {
                startedAlready = true;
            }

            return currentPath == 0;
        }
    }
    public bool DerailToWait => derailToWait;
    public bool InstantlyAccelerate => instantlyAccelerate;
    public bool Loop => loop;
    public float TargetSpeed => targetVelocity;

    public Optional<float> StartWaitTime => startWaitTime;
    public Optional<float> StartWhenDistanceFromPlayer => startWhenDistanceFromPlayer;
    public Optional<OnTriggerEvent> StartOnTriggerEnter => startOnTriggerEnter;
    public bool IsTriggerEntered => isTriggerEntered;
    private bool isTriggerEntered;

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

    public float GetClosestTValueOnCurrentPath(Vector3 currentPosition)
    {
        return GetCurrentPathObject().path.GetClosestTimeOnPath(currentPosition);
    }

    public void EnteredTrigger()
    {
        isTriggerEntered = true;
    }

    public Vector3 GetClosestPointOnCurrentPath()
    {
        Vector3 position = carts.First.Value.rb.position;
        float closestT = GetCurrentPathObject().path.GetClosestTimeOnPath(position);
        return GetCurrentPathObject().path.GetPointAtTime(closestT) + (currentRailLinker.Height * GetCurrentPathObject().path.GetNormal(closestT));
    }

    // Returns the RoadCreator object which contains the current path. Good for getting the object the path is likely on
    public PathCreation.PathCreator GetCurrentPathObject()
    {
        if (currentPath >= pathObjects.Count)
        {
            return null;
        }
        return pathObjects[currentPath];
    }

    public void SetToMaxSpeed()
    {
        foreach (Cart cart in carts)
        {
            cart.rb.AddForce(cart.rb.transform.forward * targetVelocity, ForceMode.VelocityChange);
        }
    }

    // Updates the current path to be the next one if it does exist
    public void ChangePath()
    {
        currentPath++;
        if (currentPath < pathObjects.Count)
        {
            currentRailLinker = GetCurrentPathObject().gameObject.GetComponent<TrainRailLinker>();
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
        isTriggerEntered = false;
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
        Vector3 targetVel = rot * Vector3.forward * this.TargetSpeed;
        Vector3 targetAngVel = currentVel.magnitude * (rot * TargetAngVel(rotInv * target)); //Rotate based on rotation heading

        //Target Change
        Vector3 deltaVel = targetVel - currentVel;
        Vector3 deltaAngVel = targetAngVel - currentAngVel;

        //Target Acceleration
        Vector3 acc = deltaVel / Time.fixedDeltaTime;
        Vector3 angAcc = deltaAngVel / Time.fixedDeltaTime;

        //Limit Change
        acc = rot * Vector3.Project(rotInv * acc, Vector3.forward);
        angAcc = rot * Vector3.Project(rotInv * angAcc, Vector3.up);

        //Forces
        carts.First.Value.rb.AddForce(acc, ForceMode.Acceleration);
        carts.First.Value.rb.AddTorque(angAcc, ForceMode.Acceleration);

        //Rolling Correction
        foreach (Cart cart in carts)
        {
            Vector3 upAxis = CustomGravity.GetUpAxis(cart.rb.position);
            Vector3 gravityForward = Vector3.Cross(upAxis, cart.rb.transform.right);

            Vector3 gravityUp = Vector3.ProjectOnPlane(upAxis, gravityForward);
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
        if (trainDerailed && deleteOnDerail)
        {
            timerToDelete.CountDown(Time.deltaTime);
        }

        if (!timerToDelete.TimerActive())
        {
            //RemoveCartsUntilIndex(0);
        }
    }

    public override void RestartFSM()
    {
        base.RestartFSM();
    }

    public void OnEnable()
    {
        RestartFSM();
        LevelController.PlayerRespawned += RestartFSM;
        timerToDelete = new Timer(timeBeforeDelete);
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