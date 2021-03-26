using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public interface BasicState
{
    public void SubToListeners(PlayerMachineCenter playerMachine);
    public void UnsubToListeners(PlayerMachineCenter playerMachine);
    public BasicState TransitionState(PlayerMachineCenter playerMachine);
    public void PerformBehavior(PlayerMachineCenter playerMachine);
    public bool HasSubStateMachine();
    public BasicState GetCurrentSubState();
    public BasicState[] GetSubStateArray();
}
