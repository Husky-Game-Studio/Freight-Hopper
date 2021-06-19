using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SearchState : BasicState
{
    protected TurretSubStateMachineCenter turretMachineCenter;
    public SearchState(TurretSubStateMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
    }

    // Conditions to change states
    public override BasicState TransitionState() {
        Vector3 transformOrigin = turretMachineCenter.parentMachineCenter.gameObject.transform.position;
        Vector3 transformPlayerOrigin = turretMachineCenter.parentMachineCenter.thePlayer.transform.position - transformOrigin;

        Debug.Log("turret origin: " + transformOrigin + "\n" + "Player Origin: " + transformPlayerOrigin);
        
        Ray ray = new Ray(transformOrigin, transformPlayerOrigin);

        // Debugging
        Debug.DrawRay(ray.origin, ray.direction * transformPlayerOrigin.magnitude, Color.blue);

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