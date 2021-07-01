using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMachineCenter : FiniteStateMachineCenter
{
    public InputMaster UserInputMaster => master;
    private InputMaster master;

    // The Player Character and Information
    public GameObject thePlayer;
    private Ray ray;
    public RaycastHit toPlayerRaycast;
    public LayerMask targetedLayers;
    public GameObject bullet;
    public GameObject bulletSpawner;

    // TFSM States
    public BasicState searchState;
    public BasicState targetState;
    public BasicState fireState;

    private void Awake()
    {
        CreateStatesAndFindPlayer();
    }

    private void CreateStatesAndFindPlayer()
    {
        searchState = new SearchState(this);
        targetState = new TargetState(this);
        fireState = new FireState(this);

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

    // Assign initial state and subscribe to any event listeners
    public void OnEnable()
    {
        this.currentState = searchState;
        this.previousState = searchState;
    }

    // Unsubscribe from any assigned event listeners
    public void OnDisable()
    {
        Player.PlayerLoadedIn -= SetPlayerReference;
        LevelController.PlayerRespawned -= SetPlayerReference;
    }

    public void FixedUpdate()
    {
        this.UpdateLoop();
    }

    public override void PerformStateIndependentBehaviors()
    {
        if (thePlayer == null)
        {
            SetPlayerReference();
        }
        SetRayToPlayer();
    }

    private void SetRayToPlayer()
    {
        Vector3 transformOrigin = this.gameObject.transform.position;
        Vector3 transformPlayerOrigin = this.thePlayer.transform.position - transformOrigin;
        ray = new Ray(transformOrigin, transformPlayerOrigin);
        
        //Debugging
        if (currentState == searchState)
        {
            Debug.DrawRay(ray.origin, ray.direction * transformPlayerOrigin.magnitude, Color.blue);
        } else if (currentState == targetState)
        {
            Debug.DrawRay(ray.origin, ray.direction * transformPlayerOrigin.magnitude, Color.yellow);
        }
        else // if (currentState == fireState)
        {
            Debug.DrawRay(ray.origin, ray.direction * transformPlayerOrigin.magnitude, Color.red);
        }
    }

    private void SetPlayerReference()
    {
        thePlayer = Player.Instance.transform.gameObject;
    }

    public void ShootBullet(GameObject spawner)
    {
        GameObject spawnedBullet = Instantiate(bullet, spawner.transform.position, Quaternion.identity);
        spawnedBullet.transform.LookAt(thePlayer.transform);
    }

    public Ray getRay()
    {
        return this.ray;
    }
}