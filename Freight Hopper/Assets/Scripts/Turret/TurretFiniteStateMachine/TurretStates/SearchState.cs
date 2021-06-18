using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : BasicState
{
    protected TurretSubStateMachineCenter turretMachineCenter;
    public SearchState(TurretSubStateMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
    }

    // Conditions to change states
    public override BasicState TransitionState() {
        Ray ray = new Ray(turretMachineCenter.gameObject.transform.position,
                          turretMachineCenter.parentMachineCenter.thePlayer.transform.position
                          - turretMachineCenter.gameObject.transform.position);

        // Debugging
        Debug.DrawRay(ray.origin, ray.direction * (turretMachineCenter.parentMachineCenter.thePlayer.transform.position
                                                   - turretMachineCenter.gameObject.transform.position).magnitude, Color.blue);

        // Transition to Targetting State
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, turretMachineCenter.parentMachineCenter.targetedLayers))
        {
            if (hit.rigidbody != null && hit.rigidbody.tag.Equals("Player")) {
                return turretMachineCenter.targetState;
            }
        }

        // Stay in Search State
        return this;
    }

    public override void PerformBehavior() {}
}