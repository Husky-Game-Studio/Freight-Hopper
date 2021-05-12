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
    }

    // Get other components so this gameobject to use them
    public override void OnValidate() {
        if (thePlayer == null) {
            //thePlayer = Player.Instance.transform.gameObject;
            //Debug.Log("The Player found by Turret: " + (thePlayer != null));
        }
    }

    // Assign initial state and subscribe to any event listeners
    public override void OnEnable() {
        //thePlayer = Player.Instance.transform.gameObject;
        //Debug.Log("The Player found by Turret: " + (thePlayer != null));
    }

    // Unsubscribe from any assigned event listeners
    public override void OnDisable() {}

    // Perform any behavior that is not exclusive to any one single state
    public override void PerformStateIndependentBehaviors() {
        Debug.Log("test");
        if (thePlayer == null) {
            //thePlayer = Player.Instance.transform.gameObject;
            //Debug.Log("The Player found by Turret: " + (thePlayer != null) + " on " + Time.time);
        }
    }
}

