using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMachineCenter : FiniteStateMachineCenter
{
    // The Player Character and Information
    public GameObject thePlayer;
    public RaycastHit toPlayerRaycast;
    public LayerMask targetedLayers;
    
    // TFSM States
    public BasicState searchState;
    public BasicState targetState;
    public BasicState fireState;

    // Can construct your states in Awake() or in class constructor,
    // depending if using delates for transitions. In this case
    // we are not using delates, so we can use the class constructor.
    private void Awake() {
        searchState = new SearchState(this);
        targetState = new TargetState(this);
        fireState = new FireState(this);

        if (Player.loadedIn) {
            SetPlayerReference();
        }
        else {
            Player.PlayerLoadedIn += SetPlayerReference;
        }
    }

    // Get other components to use them
    public override void OnValidate() {}

    // Assign initial state and subscribe to any event listeners
    public override void OnEnable() {
        this.currentState = searchState;
        this.previousState = searchState;
        this.currentState.EntryState();
    }

    // Unsubscribe from any assigned event listeners
    public override void OnDisable() {
        Player.PlayerLoadedIn -= SetPlayerReference;
    }

    // Perform any behavior that is not exclusive to any one single state
    public override void PerformStateIndependentBehaviors() {
        if (thePlayer == null) {
            SetPlayerReference();
        } else {
            Ray ray = new Ray(this.gameObject.transform.position, thePlayer.transform.position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, targetedLayers))
            {
                if (hit.rigidbody != null && hit.rigidbody.tag.Equals("Player")) {

                }
            }
        }
    }
    
    // Calls the loop tick
    public void FixedUpdate(){
        this.UpdateLoop();
    }

    // Sets the player reference
    private void SetPlayerReference() {
        thePlayer = Player.Instance.transform.gameObject;
    }
}