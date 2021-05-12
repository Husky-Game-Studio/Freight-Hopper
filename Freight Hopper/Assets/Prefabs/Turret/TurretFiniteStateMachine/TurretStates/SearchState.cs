using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : BasicState
{
    protected TurretMachineCenter turretMachineCenter;
    public SearchState(TurretMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
    }
    public override BasicState TransitionState() { return null; }
    public override void PerformBehavior() {}
}
