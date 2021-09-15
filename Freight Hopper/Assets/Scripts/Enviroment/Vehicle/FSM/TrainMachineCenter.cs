using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class TrainMachineCenter : FiniteStateMachineCenter
{
    // States
    public FollowPathState followPath;
    public WaitingState waiting;
    [Space]
    // Independent Data
    [SerializeField] private Optional<float> startWaitTime;
    [SerializeField] private Optional<float> startWhenDistanceFromPlayer;
    [SerializeField] private Optional<OnTriggerEvent> startOnTriggerEnter;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool instantlyAccelerate = true;
    [SerializeField] private bool spawnIn = false;

    [SerializeField] private List<PathCreation.PathCreator> pathObjects;
    [ReadOnly] public List<TrainRailLinker> railLinkers;
    [SerializeField] private float targetVelocity;
    [HideInInspector, NonSerialized] public LinkedList<Cart> carts = new LinkedList<Cart>();
    [SerializeField, ReadOnly] private bool linked = false;
    [SerializeField, ReadOnly] private int currentPath = -1;

    // Destroy Train After Derail Fields
    [SerializeField] private bool deleteOnDerail = false;
    [SerializeField] private float timeBeforeDelete = 10;

    public Action<TrainRailLinker> LinkedToPath;

    public Timer timerToDelete;
    public bool trainDerailed;
    private bool isTriggerEntered;
    private bool startedAlready = false;

    // Accessors
    public int CurrentPath => currentPath;
    public bool OnFinalPath => currentPath == pathObjects.Count - 1 && !loop;
    public TrainRailLinker GetNextRailLinker
    {
        get
        {
            if (currentPath + 1 >= railLinkers.Count)
            {
                return railLinkers[railLinkers.Count - 1];
            }

            return railLinkers[currentPath + 1];
        }
    }
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
    public bool CompletedPaths => this.OnFinalPath
        && linked
        && this.CurrentRailLinker.WithinFollowDistance(this.CurrentRailLinker.pathCreator.path.LastVertexIndex, this.Locomotive.rb.position);
    public bool InstantlyAccelerate => instantlyAccelerate;
    public bool SpawnIn => spawnIn;
    public bool Loop => loop;
    public float TargetSpeed => targetVelocity;
    public Optional<float> StartWaitTime => startWaitTime;
    public Optional<float> StartWhenDistanceFromPlayer => startWhenDistanceFromPlayer;
    public Optional<OnTriggerEvent> StartOnTriggerEnter => startOnTriggerEnter;
    public bool IsTriggerEntered => isTriggerEntered;
    public Cart Locomotive => carts.First.Value;

    // Dependecies
    [HideInInspector] public HoverController hoverController;
    public TrainRailLinker CurrentRailLinker
    {
        get
        {
            if (currentPath >= railLinkers.Count)
            {
                return railLinkers[railLinkers.Count - 1];
            }
            if (currentPath < 0)
            {
                return railLinkers[0];
            }
            return railLinkers[currentPath];
        }
    }
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

    public void EnteredTrigger()
    {
        isTriggerEntered = true;
    }

    // Returns the RoadCreator object which contains the current path. Good for getting the object the path is likely on
    public PathCreation.PathCreator GetCurrentPath()
    {
        if (currentPath >= pathObjects.Count)
        {
            return pathObjects[pathObjects.Count - 1];
        }
        return pathObjects[currentPath];
    }

    public void SetToMaxSpeed()
    {
        foreach (Cart cart in carts)
        {
            cart.rb.AddForce(cart.rb.transform.forward * targetVelocity * 0.7f, ForceMode.VelocityChange);
        }
    }

    // Updates the current path to be the next one if it does exist
    public void ChangePath()
    {
        currentPath++;
        if (loop && pathObjects.Count > 0)
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

    private void InitiliazeCarts()
    {
        PhysicsManager[] physics = GetComponentsInChildren<PhysicsManager>();

        for (int i = 0; i < physics.Length; i++)
        {
            Cart cart = new Cart(physics[i]);
            cart.destructable.RigidbodyDestroyed += cart.DestroyCartFunc;
            cart.DestoryCart += RemoveCartsUntilIndex;
            carts.AddLast(cart);
        }
        hoverController = this.Locomotive.rb.GetComponentInChildren<HoverController>();
    }

    private void Awake()
    {
        isTriggerEntered = false;
        InitiliazeCarts();

        transitionHandler = new TrainStateTransitions(this);

        // Default
        List<Func<BasicState>> defaultTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckStartState
        };
        defaultState = new DefaultState(this, defaultTransitionsList);

        // Follow Path
        List<Func<BasicState>> followPathTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckWaiting
        };
        followPath = new FollowPathState(this, followPathTransitionsList);

        // Waiting
        List<Func<BasicState>> waitingTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckFollowPath
        };
        waiting = new WaitingState(this, waitingTransitionsList);

        foreach (PathCreation.PathCreator path in pathObjects)
        {
            railLinkers.Add(path.GetComponent<TrainRailLinker>());
        }
    }

    private void OnValidate()
    {
        InitiliazeCarts();
        if (pathObjects.Count > 0 && pathObjects[0] != null)
        {
            float t = pathObjects[0].path.GetClosestTimeOnPath(this.Locomotive.rb.position);
            Vector3 position = pathObjects[0].path.GetPointAtTime(t);
            this.Locomotive.rb.transform.position = position;
            float offsetDistance = pathObjects[0].GetComponent<TrainRailLinker>().Height;
            this.Locomotive.rb.transform.position += pathObjects[0].path.GetNormal(t) * offsetDistance;
            this.Locomotive.rb.transform.rotation = pathObjects[0].path.GetRotation(t);
        }
    }

    // Only use for train starting on path
    public void LinkTrainToPath(int pathIndex)
    {
        if (this.CurrentRailLinker.IsRigidbodyLinked(this.Locomotive.rb))
        {
            return;
        }
        foreach (Cart cart in carts)
        {
            this.CurrentRailLinker.Link(cart.rb);
            if (this.Locomotive == cart)
            {
                linked = true;
                this.CurrentRailLinker.RemovedRigidbody += RemovedLocomitve;
            }
        }

        LinkedToPath?.Invoke(this.CurrentRailLinker);
    }

    public void RemovedLocomitve(Rigidbody cartRb)
    {
        if (this.Locomotive.rb == cartRb)
        {
            linked = false;
        }
    }

    // Given a position, the train will try to rotate and move towards that position
    public void Follow(Vector3 direction)
    {
        if (!hoverController.EnginesFiring)
        {
            //Debug.Log("hover engines not firing", hoverController.gameObject);
            return;
        }

        Quaternion rot = this.Locomotive.rb.transform.rotation;
        Quaternion rotInv = Quaternion.Inverse(rot);
        Vector3 currentVelDir = (this.Locomotive.rb.velocity.magnitude != 0) ? this.Locomotive.rb.velocity.normalized : Vector3.forward;
        Debug.DrawLine(this.Locomotive.rb.position, this.Locomotive.rb.position + (currentVelDir * 20.0f), Color.magenta);

        //Current
        Vector3 currentVel = this.Locomotive.rb.velocity;
        Vector3 currentAngVel = this.Locomotive.rb.angularVelocity;

        //Target (Rotate, Move Forward)
        Vector3 targetVel = rot * Vector3.forward * this.TargetSpeed;
        Vector3 targetAngVel = currentVel.magnitude * (rot * TargetAngVel(rotInv * direction)); //Rotate based on rotation heading

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
        this.Locomotive.rb.AddForce(acc, ForceMode.Acceleration);
        this.Locomotive.rb.AddTorque(angAcc, ForceMode.Acceleration);
    }

    private void KeepUpright()
    {
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
        if (!spawnIn || currentState == followPath)
        {
            KeepUpright();
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