using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireState : BasicState
{
    protected TurretMachineCenter turretMachineCenter;
    public FireState(TurretMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
    }

    // Only in this state for one tick
    public override BasicState TransitionState() {
        // Debugging
        Ray ray = new Ray(turretMachineCenter.gameObject.transform.position,
                          turretMachineCenter.thePlayer.transform.position
                          - turretMachineCenter.gameObject.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * (turretMachineCenter.thePlayer.transform.position
                                                   - turretMachineCenter.gameObject.transform.position).magnitude, Color.red);
        
        // Return to target state
        return turretMachineCenter.targetState;
    }

    public override void PerformBehavior() {
        //Debug.Log("Fire");
    }
}
