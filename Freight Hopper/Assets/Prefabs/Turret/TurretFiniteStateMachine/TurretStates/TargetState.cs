using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetState : BasicState
{
    protected TurretMachineCenter turretMachineCenter;
    public TargetState(TurretMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
    }
    // Conditions to change states
    public override BasicState TransitionState() {
        Ray ray = new Ray(turretMachineCenter.gameObject.transform.position, turretMachineCenter.thePlayer.transform.position - turretMachineCenter.gameObject.transform.position);
        //Debug.DrawRay(ray.origin, ray.direction * (turretMachineCenter.thePlayer.transform.position - turretMachineCenter.gameObject.transform.position).magnitude, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, turretMachineCenter.targetedLayers))
        {
            if ((hit.rigidbody != null && !hit.rigidbody.tag.Equals("Player")) || (hit.rigidbody == null)) {
                return turretMachineCenter.searchState;
            }
        }
        return this;
    }
    public override void PerformBehavior() {}
}
