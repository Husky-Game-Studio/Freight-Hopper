using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMachineCenter : FiniteStateMachineCenter
{
    // The Player Character and Information
    public GameObject thePlayer;

    public RaycastHit toPlayerRaycast;
    public LayerMask targetedLayers;
    public GameObject bullet;

    // TFSM States
    public BasicState testState;
    /*public BasicState searchState;
    public BasicState targetState;
    public BasicState fireState;*/

    // It is best to construct your states in Awake()
    // and subcribe to any event listeners
    private void Awake()
    {
        testState = new TestState(this);
        /*searchState = new SearchState(this);
        targetState = new TargetState(this);
        fireState = new FireState(this);*/

        if (Player.loadedIn)
        {
            SetPlayerReference();
        }
        else
        {
            Player.PlayerLoadedIn += SetPlayerReference;
        }
    }

    // Assign initial state and subscribe to any event listeners
    public void OnEnable()
    {
        /*this.currentState = searchState;
        this.previousState = searchState;*/
        
        this.currentState = testState;
        this.previousState = testState;
    }

    // Unsubscribe from any assigned event listeners
    public void OnDisable()
    {
        Player.PlayerLoadedIn -= SetPlayerReference;
    }

    // Calls the loop tick
    public void FixedUpdate()
    {
        this.UpdateLoop();
    }

    // Perform any behavior that is not exclusive to any one single state
    public override void PerformStateIndependentBehaviors()
    {
        if (thePlayer == null)
        {
            SetPlayerReference();
        }
    }

    // Sets the player reference
    private void SetPlayerReference()
    {
        thePlayer = Player.Instance.transform.gameObject;
    }

    // Fire Projectile
    public void ShootBullet(GameObject spawner)
    {
        GameObject spawnedBullet = Instantiate(bullet, spawner.transform.position, Quaternion.identity);
        spawnedBullet.transform.LookAt(thePlayer.transform);
    }
}