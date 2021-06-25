using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SearchState : BasicState
{
    protected TurretMachineCenter turretMachineCenter;
    public SearchState(TurretMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
    }

    // Conditions to change states
    public override BasicState TransitionState() {
        Vector3 transformOrigin = turretMachineCenter.gameObject.transform.position;
        Vector3 transformPlayerOrigin = turretMachineCenter.thePlayer.transform.position - transformOrigin;
        
        Ray ray = new Ray(transformOrigin, transformPlayerOrigin);

        // Debugging
        Debug.DrawRay(ray.origin, ray.direction * transformPlayerOrigin.magnitude, Color.blue);

        // Transition to Targetting State
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, turretMachineCenter.targetedLayers))
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