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

        
        myMachine.gameObject.AddComponent<TurretSubStateMachineCenter>().parentMachineCenter = (TurretMachineCenter)myMachine;
        
        mySubCenter = myMachine.gameObject.GetComponent<TurretSubStateMachineCenter>();
        
        /*public static TurretSubStateMachineCenter CreateComponent (GameObject this.myMachine.gameObject, FiniteStateMachineCenter myMachine) {
            TurretSubStateMachineCenter subCenter = this.myMachine.gameObject.AddComponent<TurretSubStateMachineCenter>();
            subCenter.parentMachineCenter = myMachine;
            return subCenter;
        }*/
        
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
