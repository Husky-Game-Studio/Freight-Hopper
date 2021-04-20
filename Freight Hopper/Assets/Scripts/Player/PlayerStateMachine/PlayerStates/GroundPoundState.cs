using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPoundState : BasicState
{
    private PlayerSubStateMachineCenter pSSMC;
    private PlayerMachineCenter myPlayerMachineCenter;
    private BasicState[] miniStateArray;

    public void SubToListeners(FiniteStateMachineCenter machineCenter) {

        
    }
    public void UnsubToListeners(FiniteStateMachineCenter machineCenter) {


    }
    public BasicState TransitionState(FiniteStateMachineCenter machineCenter) {

        return null;
    }
    public void PerformBehavior(FiniteStateMachineCenter machineCenter) {}
    public bool HasSubStateMachine() { return true; }
    public BasicState GetCurrentSubState() { return pSSMC.GetCurrentSubState(); }
    public BasicState[] GetSubStateArray() { return miniStateArray; }
}
