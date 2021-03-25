using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public interface BasicState
{
    void subToListeners(PlayerMachineCenter playerMachine);
    void unsubToListeners(PlayerMachineCenter playerMachine);
    BasicState DoState(PlayerMachineCenter playerMachine);
}
