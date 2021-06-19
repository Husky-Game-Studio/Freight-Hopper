using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestState : BasicState
{
    private TurretSubStateMachineCenter mySubCenter;
    private FiniteStateMachineCenter myMachine;
    
    public TestState(FiniteStateMachineCenter myMachine) : base(myMachine)
    {
        this.myMachine = myMachine;
        myMachine.gameObject.AddComponent<TurretSubStateMachineCenter>().parentMachineCenter = (TurretMachineCenter)myMachine;
        mySubCenter = myMachine.gameObject.GetComponent<TurretSubStateMachineCenter>();
    } 
    
    public override BasicState TransitionState()
    {
        return this;
    }

    public override void PerformBehavior()
    {
        mySubCenter.UpdateLoop();
    }
}
