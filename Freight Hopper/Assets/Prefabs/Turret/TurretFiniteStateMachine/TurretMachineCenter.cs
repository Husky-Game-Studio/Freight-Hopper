using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMachineCenter : FiniteStateMachineCenter
{
    // The Player Character, set in the inspector
    public GameObject thePlayer;
    
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
            setPlayerReference();
        }
        else {
            Player.PlayerLoadedIn += setPlayerReference;
        }
    }

    // Get other components to use them
    public override void OnValidate() {
        
    }

    // Assign initial state and subscribe to any event listeners
    public override void OnEnable() {
        //thePlayer = Player.Instance.transform.gameObject;
        //Debug.Log("The Player found by Turret: " + (thePlayer != null));
        this.currentState = searchState;
        this.previousState = searchState;
        this.currentState.EntryState();
    }

    // Unsubscribe from any assigned event listeners
    public override void OnDisable() {
        Player.PlayerLoadedIn -= setPlayerReference;
    }

    private void setPlayerReference() {
        thePlayer = Player.Instance.transform.gameObject;
    }

    // Perform any behavior that is not exclusive to any one single state
    public override void PerformStateIndependentBehaviors() {
        if (thePlayer == null) {
            setPlayerReference();
        }
    }
    
    public void FixedUpdate(){
        this.UpdateLoop();
    }
}