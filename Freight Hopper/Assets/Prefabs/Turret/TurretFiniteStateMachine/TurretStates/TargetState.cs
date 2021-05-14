using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Help from this video for rotation: https://www.youtube.com/watch?v=nJiFitClnKo

public class TargetState : BasicState
{
    protected TurretMachineCenter turretMachineCenter;
    private Transform thePlayerTransform;
    private Transform turretTransform;
    private Transform barrelBaseTransform;
    private float speedOfRotation = 5f;
    private Timer countDownToTimer;

    public TargetState(TurretMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
        //thePlayerTransform = turretMachineCenter.thePlayer.transform;
        turretTransform = turretMachineCenter.gameObject.transform;
        barrelBaseTransform = turretTransform.Find("Barrel_Base").transform;
        countDownToTimer = new Timer(2f);
    }

    // Conditions to change states
    public override BasicState TransitionState() {
        // Debugging
        Ray ray = new Ray(turretMachineCenter.gameObject.transform.position,
                          turretMachineCenter.thePlayer.transform.position
                          - turretMachineCenter.gameObject.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * (turretMachineCenter.thePlayer.transform.position
                                                   - turretMachineCenter.gameObject.transform.position).magnitude, Color.yellow);

        // Transition to Search State
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, turretMachineCenter.targetedLayers))
        {
            if ((hit.rigidbody != null && !hit.rigidbody.tag.Equals("Player")) || (hit.rigidbody == null)) {
                return turretMachineCenter.searchState;
            }
        }
        // Transition to Fire State
        if (!countDownToTimer.TimerActive())
        {
            return turretMachineCenter.fireState;
        }
        // Stay in Targeting State
        return this;
    }

    public override void EntryState()
    {
        countDownToTimer.ResetTimer();
    }

    // Rotate Turret to aim at player
    public override void PerformBehavior() {
        thePlayerTransform = turretMachineCenter.thePlayer.transform;
        //turretTransform = turretMachineCenter.gameObject.transform;
        //barrelBaseTransform = turretTransform.Find("Barrel_Base").transform;

        // Turn whole turret to follow the player on the xz-plane
        Vector3 direction = (thePlayerTransform.position - turretTransform.position);
        Quaternion xzRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
        turretTransform.rotation
                = Quaternion.Lerp(turretTransform.rotation, xzRotation, speedOfRotation * Time.fixedDeltaTime);

        // Turn turret barrel to follow the player on the y-axis
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z), Vector3.up);
        barrelBaseTransform.rotation
                = Quaternion.Lerp(barrelBaseTransform.rotation, rotation, speedOfRotation * Time.fixedDeltaTime);

        countDownToTimer.CountDownFixed();
    }

    public override void ExitState()
    {
        countDownToTimer.DeactivateTimer();
    }
}
