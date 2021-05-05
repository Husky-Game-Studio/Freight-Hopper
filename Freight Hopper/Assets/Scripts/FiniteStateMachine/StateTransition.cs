using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Help developing this and related code using this YouTube video: https://www.youtube.com/watch?v=V75hgcsCGOM&t=944s
// Delagates (func), actions, or function pointers to store the functions.

public interface StateTransition
{
    public bool shouldTransition(FiniteStateMachineCenter machineCenter);
}