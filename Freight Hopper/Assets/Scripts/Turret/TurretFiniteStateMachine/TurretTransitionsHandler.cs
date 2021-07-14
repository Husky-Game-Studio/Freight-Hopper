using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTransitionsHandler 
{
    private TurretMachineCenter turretFSM;

    public TurretTransitionsHandler(FiniteStateMachineCenter machineCenter)
    {
        turretFSM = (TurretMachineCenter)machineCenter;
    }

    public BasicState CheckStartState()
    {
        if (Player.loadedIn)
        {
            return turretFSM.searchState;
        }
        return null;
    }
    
    public BasicState CheckTargetState()
    {
        if (Physics.Raycast(turretFSM.GetRay(), out RaycastHit hit, Mathf.Infinity, turretFSM.targetedLayers))
        {
            if (hit.rigidbody != null && hit.rigidbody.CompareTag("Player")) {
                return turretFSM.targetState;
            }
        }
        return null;
    }
    
    public BasicState CheckSearchState()
    {
        if (Physics.Raycast(turretFSM.GetRay(), out RaycastHit hit, Mathf.Infinity, turretFSM.targetedLayers))
        {
            if ((hit.rigidbody != null && !hit.rigidbody.CompareTag("Player")) || (hit.rigidbody == null))
            {
                return turretFSM.searchState;
            }
        }
        return null;
    }
}
