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
    private float speedOfRotation = 5f;
    [SerializeField]private GameObject landingIndicator;

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
        if (currentState == targetState)
        {
            SetBarrelAngle(CalculateLaunchData());
        }
        if (debugPath)
        {
            DrawPath();
        }
    }

    private void RayCastToTarget()
    {
        Vector3 transformOrigin = barrelBase.transform.position;//this.gameObject.transform.position;
        Vector3 transformTargetOrigin = this.theTarget.transform.position - transformOrigin;
        ray = new Ray(transformOrigin, transformTargetOrigin);
        
        //Debugging
        if (debugPath)
        {
            if (currentState == searchState)
            {
                Debug.DrawRay(ray.origin, ray.direction * transformTargetOrigin.magnitude, Color.blue);
            }
            else if (currentState == targetState)
            {
                Debug.DrawRay(ray.origin, ray.direction * transformTargetOrigin.magnitude, Color.yellow);
            }
            else if (currentState == fireState)
            {
                Debug.DrawRay(ray.origin, ray.direction * transformTargetOrigin.magnitude, Color.red);
            }
        }
    }

    private void SetPlayerReference()
    {
        theTarget = Player.Instance.transform.gameObject;
    }

    public void ShootBullet(GameObject spawner)
    {
        //Vector3 spawnAngle = barrelBase.transform.forward;
        GameObject spawnedBullet = Instantiate(bullet, spawner.transform.position, Quaternion.identity);
        spawnedBullet.transform.LookAt(theTarget.transform);
        SetProjectileForce(projectileForce);
        SetTargetPosition(theTarget.transform.position);
        LaunchSequence(spawnedBullet);
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
    
    
    
    /*
     * Bullet Path Calculating
     */
    
    
    
    private Rigidbody projectileRB;
    private Rigidbody turretRB;
    private float projForce = 0f;
    private Vector3 targetPosition;
    
    // maximumVerticleDisplacement must be less than the gravity to work
    private float maximumVerticleDisplacement = 25f;
    [SerializeField]private float arcIntensity = 5f;
    private float gravity;

    [SerializeField]private bool debugPath = true;
    
    private void LaunchSequence(GameObject spawnedProjectile)
    {
        turretRB = this.gameObject.GetComponent<Rigidbody>();
        //Destroy(spawnedProjectile, 4f);
        projectileRB = spawnedProjectile.GetComponent<Rigidbody>();
        targetPosition = theTarget.transform.position;
        //myRB.AddForce(this.gameObject.transform.forward.normalized * projectileForce, ForceMode.Impulse);
        gravity = Physics.gravity.y;
        Launch();
    }

    private void SetLandingIndicator(LaunchData myLD)
    {
        GameObject landingIndicator = Instantiate(this.landingIndicator, theTarget.transform.position + (theTarget.transform.localScale/2), new Quaternion(0, 1, 0, 1));
        Destroy(landingIndicator, myLD.timeToTarget);
    }
    
    private void Launch()
    {
        LaunchData myLD = CalculateLaunchData();
        projectileRB.velocity = myLD.initialVeloctiy;
        SetLandingIndicator(myLD);
    }
    
    private LaunchData CalculateLaunchData()
    {
        if (this.gameObject.transform.position.y < targetPosition.y)
        {
            maximumVerticleDisplacement = targetPosition.y + arcIntensity;
        }
        else
        {
            maximumVerticleDisplacement = 0.1f;
        }

        float displacementY = targetPosition.y - bulletSpawner.transform.position.y;
        Vector3 displacementXZ =
            new Vector3(targetPosition.x - bulletSpawner.transform.position.x, 0, targetPosition.z - bulletSpawner.transform.position.z);
        float time =
            Mathf.Sqrt(-2 * maximumVerticleDisplacement / gravity) + Mathf.Sqrt(2 * (displacementY - maximumVerticleDisplacement) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * maximumVerticleDisplacement);
        Vector3 velocityXZ = displacementXZ / time;
        
        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    private void SetBarrelAngle(LaunchData myLD)
    {
        Vector3 direction = myLD.initialVeloctiy;
        Quaternion turretRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, turretRotation, speedOfRotation * Time.fixedDeltaTime);
        Quaternion barrelRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z), Vector3.up);
        barrelBase.transform.rotation = Quaternion.Lerp(barrelBase.transform.rotation, barrelRotation, speedOfRotation * Time.fixedDeltaTime);
    }

    private void DrawPath()
    {
        SetTargetPosition(theTarget.transform.position);
        LaunchData launchData = CalculateLaunchData();
        Vector3 previousDrawPoint = bulletSpawner.transform.position;

        int resolution = 70;
        for (int i = 0; i < resolution; i++)
        {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 displacement = launchData.initialVeloctiy * simulationTime + Vector3.up *
                                   gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = bulletSpawner.transform.position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }
    }

    struct LaunchData
    {
        public readonly Vector3 initialVeloctiy;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVeloctiy, float timeToTarget)
        {
            this.initialVeloctiy = initialVeloctiy;
            this.timeToTarget = timeToTarget;
        }
        
    }
    
     public void SetTargetPosition(Vector3 position)
     {
         targetPosition = position;
     }

     public void SetProjectileForce(float force)
     {
         projForce = force;
     }
     
    
    
    
    
    
    
    
    
    
    
    
    
    
}