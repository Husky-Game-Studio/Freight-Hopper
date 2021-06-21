using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSubStateMachineCenter : FiniteStateMachineCenter
{
    public TurretMachineCenter parentMachineCenter;
    
    public BasicState searchState;
    public BasicState targetState;
    public BasicState fireState;

    public TurretSubStateMachineCenter(FiniteStateMachineCenter parentMachineCenter)
    {
        this.parentMachineCenter = (TurretMachineCenter)parentMachineCenter;
        this.SetAsSubStateMachine();
    }

    private void Awake()
    {
        /*searchState = new SearchState(this);
        targetState = new TargetState(this);
        fireState = new FireState(this);
        if (this.amSubStateMachine)
        {
            Debug.Log("SubStateMachineActivated");
        }*/
    }
    
    public void OnEnable()
    {
        this.currentState = searchState;
        this.previousState = searchState;
    }

    private void Start()
    {
        searchState = new SearchState(this);
        targetState = new TargetState(this);
        fireState = new FireState(this);
        if (this.amSubStateMachine)
        {
            Debug.Log("SubStateMachineActivated");
        }
        
        this.currentState = searchState;
        this.previousState = searchState;
    }
}
