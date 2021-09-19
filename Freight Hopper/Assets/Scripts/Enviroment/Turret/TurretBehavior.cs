using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    [SerializeField] private bool debugging;
    [SerializeField] private Weapon weapon;
    [SerializeField] private bool targetPlayer;
    [SerializeField] private Transform[] targets;
    [SerializeField] private Rigidbody[] targetRbs;
    [SerializeField] private Vector3 relativeTargetOffset;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private Transform body;
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform barrelEnd;
    [SerializeField] private Rigidbody turretRb;
    [SerializeField] private OnTriggerEvent[] enableTriggers;
    [SerializeField] private OnTriggerEvent[] disableTriggers;
    [SerializeField, Tooltip("timers alternate from disabling/enabling the turret")] private Timer[] enableDisableTimers;
    [SerializeField, ReadOnly] private float fireRange = 250;
    [SerializeField, ReadOnly] private Timer rangeCheckTime = new Timer(1);
    [SerializeField, ReadOnly] private int currentTarget;
    [SerializeField, ReadOnly] private bool firing = true;
    private const float rotationSpeed = 1000;
    private Vector3 targetPrediction;

    private void OnValidate()
    {
    }

    private void Awake()
    {
        firing = true;
        if (targetPlayer)
        {
            if (Player.loadedIn)
            {
                SetPlayerReference();
            }
            else
            {
                Player.PlayerLoadedIn += SetPlayerReference;
                LevelController.PlayerRespawned += SetPlayerReference;
            }
        }
    }

    private void SetPlayerReference()
    {
        targets[0] = Player.Instance.transform;
        if (targets[0] != null)
        {
            targetRbs[0] = targets[0].GetComponent<PhysicsManager>().rb;
        }
    }

    private void OnDisable()
    {
        if (targetPlayer)
        {
            Player.PlayerLoadedIn -= SetPlayerReference;
            LevelController.PlayerRespawned -= SetPlayerReference;
        }
    }

    private void FixedUpdate()
    {
        if (targetPlayer)
        {
            if (targets.Length < 1 || targets[0] == null)
            {
                return;
            }
        }

        if (firing)
        {
            if (!rangeCheckTime.TimerActive())
            {
                float height = Vector3.Project(body.position - targets[currentTarget].position, CustomGravity.GetUpAxis(body.position)).magnitude;
                fireRange = Ballistics.ballistic_range(weapon.ProjectileSpeed, CustomGravity.GetGravity(body.position).magnitude, height);
                rangeCheckTime.ResetTimer();
            }
            rangeCheckTime.CountDown(Time.fixedDeltaTime);

            if (Vector3.Distance(targetPrediction, body.position) < fireRange)
            {
                targetPrediction = CalculateTarget();
                Debug.DrawLine(barrelEnd.position, barrelEnd.position + targetPrediction, Color.red);
            }

            RotateBarrel(targetPrediction);

            RotateBody();
            if (weapon.CanFire)
            {
                Debug.DrawRay(barrelEnd.position, (targets[currentTarget].position - barrelEnd.position).normalized * 100, Color.green);
                if (Physics.Raycast(barrelEnd.position, (targets[currentTarget].position - barrelEnd.position).normalized, out RaycastHit hit, targetLayers))
                {
                    if (hit.collider.gameObject.transform != targets[currentTarget])
                    {
                        return;
                    }
                }
                weapon.Fire(barrelEnd.position, barrelEnd.forward, targetPrediction);
            }
        }
    }

    private Vector3 CalculateTarget()
    {
        Vector3 targetPos = targets[currentTarget].position;
        Vector3 s0;
        float gravityMag = CustomGravity.GetGravity(targetPos).magnitude;
        int solutions;

        if (targetRbs.Length <= currentTarget || targetRbs[currentTarget] == null)
        {
            solutions = Ballistics.solve_ballistic_arc(barrelEnd.position, weapon.ProjectileSpeed, targetPos, gravityMag, out s0, out _);
        }
        else
        {
            Vector3 targetVelocity = targetRbs[currentTarget].velocity;
            Vector3 s1;
            solutions = Ballistics.solve_ballistic_arc(barrelEnd.position, weapon.ProjectileSpeed, targetPos, targetVelocity, gravityMag, out s0, out s1);
        }
        if (solutions == 0)
        {
            //Debug.Log(this.name + " projectile speed too slow to hit target");
            return targetPos;
        }

        return s0;
    }

    // From https://answers.unity.com/questions/636145/how-rotate-turret-and-follow-body-object.html
    private void RotateBody()
    {
        Vector3 targetPos = targets[currentTarget].position;
        Vector3 up = body.up;
        float distanceToPlane = Vector3.Dot(up, targetPos - body.position);
        Vector3 planePoint = targetPos - (up * distanceToPlane);

        Quaternion qTurret = Quaternion.LookRotation(planePoint - body.position, up);
        body.rotation = Quaternion.RotateTowards(body.rotation, qTurret, rotationSpeed * Time.fixedDeltaTime);
        //body.rotation = Quaternion.LookRotation(target, up);
    }

    private void RotateBarrel(Vector3 target)
    {
        Vector3 up = body.right;
        if (target.IsZero() || up.IsZero())
        {
            return;
        }
        //float distance = Vector3.Distance(barrelEnd.position, target);
        //float gravity = CustomGravity.GetGravity(body.position).magnitude;
        //float degrees = Mathf.Rad2Deg * 1f / 2f * Mathf.Asin((gravity * distance) / (weapon.ProjectileSpeed * weapon.ProjectileSpeed));
        //
        //Debug.Log("we need to shoot a theta of " + degrees);
        Vector3 targetPos = target;

        float distanceToPlane = Vector3.Dot(up, targetPos - barrel.position);
        Vector3 planePoint = targetPos - (up * distanceToPlane);
        //Quaternion.Euler(degrees, 0, 0) *
        Quaternion qTurret = Quaternion.LookRotation((planePoint - barrel.position), up);
        //qTurret = Quaternion.LookRotation(Quaternion.Euler(theta, 0, 0), up);
        //barrel.rotation = qTurret;
        barrel.rotation = Quaternion.LookRotation(targetPos, up);
        //barrel.rotation = Quaternion.RotateTowards(barrel.rotation, Quaternion.Euler(target), rotationSpeed * Time.fixedDeltaTime);
    }
}