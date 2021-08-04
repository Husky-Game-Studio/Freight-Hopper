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
    public GameObject bullet;
    public GameObject bulletSpawner;

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
        Vector3 transformOrigin = this.gameObject.transform.position;
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
}