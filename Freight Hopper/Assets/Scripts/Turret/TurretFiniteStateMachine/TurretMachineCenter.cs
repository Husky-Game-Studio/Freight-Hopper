using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TurretMachineCenter : FiniteStateMachineCenter
{
    public InputMaster UserInputMaster => master;
    private InputMaster master;

    // The Target and Information
    [FormerlySerializedAs("targetPlayer")] [SerializeField]private bool targetingPlayer = false;
    [FormerlySerializedAs("thePlayer")] [SerializeField]private GameObject theTarget;
    private Ray ray;
    //public RaycastHit toPlayerRaycast;
    public LayerMask targetedLayers;
    [SerializeField]private GameObject bullet;
    [SerializeField]private float projectileForce = 20;
    public GameObject bulletSpawner;
    private GameObject barrelBase;

    [SerializeField] private Optional<OnTriggerEvent> startOnTriggerEnter;
    public Optional<OnTriggerEvent> StartOnTriggerEnter => startOnTriggerEnter;
    public bool IsTriggerEntered => isTriggerEntered;
    private bool isTriggerEntered;

    private TurretTransitionsHandler turretTransitionsHandler;
    //private bool playerNotSpawned = true;

    // TFSM States
    public BasicState searchState;
    public BasicState targetState;
    public BasicState fireState;

    private void Awake()
    {
        turretTransitionsHandler = new TurretTransitionsHandler(this);
        
        // Default
        List<Func<BasicState>> defaultTransitionsList = new List<Func<BasicState>>
        {
            turretTransitionsHandler.CheckStartState
        };
        defaultState = new DefaultState(this, defaultTransitionsList);
        
        List<Func<BasicState>> searchTransitionsList = new List<Func<BasicState>>
        {
            turretTransitionsHandler.CheckTargetState
        };
        searchState = new SearchState(this, searchTransitionsList);
        
        List<Func<BasicState>> targetTransitionsList = new List<Func<BasicState>>
        {
            turretTransitionsHandler.CheckSearchState
        };
        targetState = new TargetState(this, targetTransitionsList);
        
        CreateStatesAndFindPlayer();
        
        //RestartFSM();
        currentState = defaultState;
        previousState = defaultState;

        barrelBase = this.gameObject.transform.GetChild(1).gameObject;
    }

    private void CreateStatesAndFindPlayer()
    {
        //searchState = new SearchState(this);
        //targetState = new TargetState(this);
        fireState = new FireState(this);

        if (targetingPlayer)
        {
            if (Player.loadedIn)
            {
                SetPlayerReference();
            }
            else
            {
                Player.PlayerLoadedIn += SetPlayerReference;
                LevelController.PlayerRespawned += SetPlayerReference;
            }
        }
    }

    // Assign initial state and subscribe to any event listeners
    public void OnEnable()
    {

    }

    // Unsubscribe from any assigned event listeners
    public void OnDisable()
    {
        theTarget = null;
        currentState = defaultState;
        if (targetingPlayer)
        {
            Player.PlayerLoadedIn -= SetPlayerReference;
            LevelController.PlayerRespawned -= SetPlayerReference;
        }
    }

    public void FixedUpdate()
    {
        this.UpdateLoop();
    }

    public override void PerformStateIndependentBehaviors()
    {
        if (theTarget == null)
        {
            if (targetingPlayer)
            {
                SetPlayerReference();
            }
        }
        else
        {
            //this.currentState = searchState;
            //this.previousState = searchState;
        }
        RayCastToTarget();
    }

    private void RayCastToTarget()
    {
        Vector3 transformOrigin = barrelBase.transform.position;//this.gameObject.transform.position;
        Vector3 transformTargetOrigin = this.theTarget.transform.position - transformOrigin;
        ray = new Ray(transformOrigin, transformTargetOrigin);
        
        //Debugging
        if (currentState == searchState)
        {
            Debug.DrawRay(ray.origin, ray.direction * transformTargetOrigin.magnitude, Color.blue);
        } else if (currentState == targetState)
        {
            Debug.DrawRay(ray.origin, ray.direction * transformTargetOrigin.magnitude, Color.yellow);
        }
        else if (currentState == fireState)
        {
            Debug.DrawRay(ray.origin, ray.direction * transformTargetOrigin.magnitude, Color.red);
        }
    }

    private void SetPlayerReference()
    {
        theTarget = Player.Instance.transform.gameObject;
    }

    public void ShootBullet(GameObject spawner)
    {
        GameObject spawnedBullet = Instantiate(bullet, spawner.transform.position, Quaternion.identity);
        spawnedBullet.transform.LookAt(theTarget.transform);
        spawnedBullet.GetComponent<BulletBehavior>().projectileForce = projectileForce;
    }
    public void ShootBullet()
    {
        ShootBullet(bulletSpawner);
    }

    public Ray GetRay()
    {
        return this.ray;
    }


    public GameObject getTarget()
    {
        return theTarget;
    }
    
    public bool isTargetingPlayer()
    {
        return targetingPlayer;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("OnCollisionEnter");
        if (other.gameObject.GetComponent<OnTriggerEvent>() != null)
        {
            setFireTick(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (other.gameObject.GetComponent<OnTriggerEvent>() != null)
        {
            setFireTick(true);
        }
    }


    private bool fireTick = false;
    public void setFireTick(bool fireBool) { fireTick = fireBool; }
    public bool getFireTick() { return fireTick; }
}