using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public interface BasicState
{
    void SubToListeners(PlayerMachineCenter playerMachine);
    void UnsubToListeners(PlayerMachineCenter playerMachine);
    BasicState TransitionState(PlayerMachineCenter playerMachine);
    void PerformBehavior(PlayerMachineCenter playerMachine);


    // Need an entry behavior

    // Need an exit behavior

}
