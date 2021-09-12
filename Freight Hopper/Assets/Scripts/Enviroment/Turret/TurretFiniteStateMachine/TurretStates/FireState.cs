using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireState : BasicState
{
    protected TurretMachineCenter turretMachineCenter;
    private GameObject bulletSpawner;

    public FireState(TurretMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
        bulletSpawner = turretMachineCenter.bulletSpawner;
    }
    
    // Only in this state for one tick
    public override BasicState TransitionState()
    {
        return turretMachineCenter.targetState;
    }

    public override void PerformBehavior()
    {
        turretMachineCenter.ShootBullet(bulletSpawner);
    }
}