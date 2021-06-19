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

        /*mySubCenter = myMachine.gameObject.AddComponent(typeof(TurretSubStateMachineCenter)) as TurretSubStateMachineCenter;
        mySubCenter.TurretSubStateMachineCenter(myMachine);*/

        mySubCenter = myMachine.gameObject.AddComponent<TurretSubStateMachineCenter>();
        //mySubCenter.TurretSubSTateMachineCenter();
        
        //myMachine.gameObject.AddComponent(TurretSubStateMachineCenter(myMachine));
        //mySubCenter = new TurretSubStateMachineCenter(myMachine);
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
