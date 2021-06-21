using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireState : BasicState
{
    protected TurretSubStateMachineCenter turretMachineCenter;
    private GameObject bulletSpawner;

    public FireState(TurretSubStateMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;

        bulletSpawner =
                turretMachineCenter.gameObject.transform.Find("Barrel_Base").transform.Find("Turret_Barrel").transform.Find("Bullet_Spawner").gameObject;
    }

    // Only in this state for one tick
    public override BasicState TransitionState()
    {
        // Debugging
        Ray ray = new Ray(turretMachineCenter.parentMachineCenter.gameObject.transform.position, turretMachineCenter.parentMachineCenter.thePlayer.transform.position - turretMachineCenter.parentMachineCenter.gameObject.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * (turretMachineCenter.parentMachineCenter.thePlayer.transform.position - turretMachineCenter.parentMachineCenter.gameObject.transform.position).magnitude, Color.red);

        // Return to target state
        return turretMachineCenter.targetState;
    }

    // Fire Projectile
    public override void PerformBehavior()
    {
        turretMachineCenter.parentMachineCenter.ShootBullet(bulletSpawner);
    }
}