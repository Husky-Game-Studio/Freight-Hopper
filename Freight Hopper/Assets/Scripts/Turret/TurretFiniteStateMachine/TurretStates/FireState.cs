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

        bulletSpawner =
                turretMachineCenter.gameObject.transform.Find("Barrel_Base").transform.Find("Turret_Barrel").transform.Find("Bullet_Spawner").gameObject;
    }

    // Only in this state for one tick
    public override BasicState TransitionState()
    {
        // Debugging
        Ray ray = new Ray(turretMachineCenter.gameObject.transform.position, turretMachineCenter.thePlayer.transform.position - turretMachineCenter.gameObject.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * (turretMachineCenter.thePlayer.transform.position - turretMachineCenter.gameObject.transform.position).magnitude, Color.red);

        // Return to target state
        return turretMachineCenter.targetState;
    }

    // Fire Projectile
    public override void PerformBehavior()
    {
        turretMachineCenter.ShootBullet(bulletSpawner);
    }
}