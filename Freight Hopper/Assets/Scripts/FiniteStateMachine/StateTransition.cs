using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface StateTransition
{
    public bool shouldTransition(FiniteStateMachineCenter machineCenter);
}
