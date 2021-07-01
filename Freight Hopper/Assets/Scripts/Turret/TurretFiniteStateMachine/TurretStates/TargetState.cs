using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

// Help from this video for rotation: https://www.youtube.com/watch?v=nJiFitClnKo

public class TargetState : BasicState
{
    protected TurretMachineCenter turretMachineCenter;
    private Transform thePlayerTransform;
    private Transform turretTransform;
    private Transform barrelBaseTransform;
    private float speedOfRotation = 5f;
    private Timer countDownToTimer = new Timer(2f);

    public TargetState(TurretMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
        turretTransform = turretMachineCenter.gameObject.transform;
        barrelBaseTransform = turretTransform.Find("Barrel_Base").transform;
    }

    public override BasicState TransitionState()
    {
        // Transition to Search State
        if (Physics.Raycast(turretMachineCenter.getRay(), out RaycastHit hit, Mathf.Infinity, turretMachineCenter.targetedLayers))
        {
            if ((hit.rigidbody != null && !hit.rigidbody.CompareTag("Player")) || (hit.rigidbody == null))
            {
                return turretMachineCenter.searchState;
            }
        }
        // Transition to Fire State
        if (!countDownToTimer.TimerActive())
        {
            return turretMachineCenter.fireState;
        }
        return this;
    }

    public override void EntryState()
    {
        countDownToTimer.ResetTimer();
    }

    // Rotate Turret to aim at player
    public override void PerformBehavior()
    {
        thePlayerTransform = turretMachineCenter.thePlayer.transform;
        Vector3 direction = thePlayerTransform.position - turretTransform.position;
        RotateTurretBody(direction);
        ChangeBarrelYAngle(direction);
        countDownToTimer.CountDownFixed();
    }

    private void RotateTurretBody(Vector3 direction)
    {
        Quaternion xzRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
        turretTransform.rotation = Quaternion.Lerp(turretTransform.rotation, xzRotation, speedOfRotation * Time.fixedDeltaTime); 
    }

    private void ChangeBarrelYAngle(Vector3 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z), Vector3.up);
        barrelBaseTransform.rotation = Quaternion.Lerp(barrelBaseTransform.rotation, rotation, speedOfRotation * Time.fixedDeltaTime);
    }

    public override void ExitState()
    {
        countDownToTimer.DeactivateTimer();
    }
}