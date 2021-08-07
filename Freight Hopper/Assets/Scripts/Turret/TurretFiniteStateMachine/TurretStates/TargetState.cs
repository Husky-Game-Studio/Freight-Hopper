using System;
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
    private Transform theTargetTransform;
    private Transform turretTransform;
    private Transform barrelBaseTransform;
    private float speedOfRotation = 5f;
    public Timer countDownToTimer = new Timer(2f);
    public Timer CountDownToTimer => countDownToTimer;

    public TargetState(TurretMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
        ContructorHelper(turretMachineCenter);
    }

    public TargetState(TurretMachineCenter turretMachineCenter, List<Func<BasicState>> stateTransitions) : base(turretMachineCenter, stateTransitions)
    {
        ContructorHelper(turretMachineCenter);
    }

    private void ContructorHelper(TurretMachineCenter turretMachineCenter)
    {
        this.turretMachineCenter = turretMachineCenter;
        turretTransform = turretMachineCenter.gameObject.transform;
        barrelBaseTransform = turretTransform.Find("Barrel_Base").transform;
    }

    public override BasicState TransitionState()
    {
        // Transition to Search State
        /*if (Physics.Raycast(turretMachineCenter.GetRay(), out RaycastHit hit, Mathf.Infinity, turretMachineCenter.targetedLayers))
        {
            if ((hit.rigidbody != null && !hit.rigidbody.CompareTag("Player")) || (hit.rigidbody == null))
            {
                return turretMachineCenter.searchState;
            }
        }*/
        // Transition to Fire State
        if (!countDownToTimer.TimerActive())
        {
            return turretMachineCenter.fireState;
        }
        BasicState state = CheckTransitions();
        return state;
    }

    public override void EntryState()
    {
        theTargetTransform = turretMachineCenter.getTarget().transform;
        countDownToTimer.ResetTimer();
    }

    // Rotate Turret to aim at player
    public override void PerformBehavior()
    {
        Vector3 direction = theTargetTransform.position - turretTransform.position;
        RotateToTarget(direction);
        countDownToTimer.CountDown(Time.fixedDeltaTime);
    }

    private void RotateToTarget(Vector3 direction)
    {
        Quaternion turretRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
        turretTransform.rotation = Quaternion.Lerp(turretTransform.rotation, turretRotation, speedOfRotation * Time.fixedDeltaTime);
        Quaternion barrelRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z), Vector3.up);
        barrelBaseTransform.rotation = Quaternion.Lerp(barrelBaseTransform.rotation, barrelRotation, speedOfRotation * Time.fixedDeltaTime);
    }

    public override void ExitState()
    {
        countDownToTimer.DeactivateTimer();
    }
}