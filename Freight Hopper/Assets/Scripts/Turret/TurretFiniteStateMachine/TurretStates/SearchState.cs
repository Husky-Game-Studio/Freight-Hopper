using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SearchState : BasicState
{
    protected TurretMachineCenter turretMachineCenter;
    public SearchState(TurretMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
    }

    public override BasicState TransitionState() {
        // Transition to Targetting State
        if (Physics.Raycast(turretMachineCenter.GetRay(), out RaycastHit hit, Mathf.Infinity, turretMachineCenter.targetedLayers))
        {
            if (hit.rigidbody != null && hit.rigidbody.CompareTag("Player")) {
                return turretMachineCenter.targetState;
            }
        }
        return this;
    }

    public override void PerformBehavior() {}
}