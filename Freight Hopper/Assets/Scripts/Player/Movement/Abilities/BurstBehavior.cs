using UnityEngine;
using UnityEngine.VFX;

public class BurstBehavior : AbilityBehavior
{
    [SerializeField] private Transform burstExplosionEffectTransform;
    [SerializeField] private VisualEffect burstExplosionEffect;
    [SerializeField] private float burstMaxDistance = 1.5f;
    [SerializeField] private LayerMask targetedLayers;
    [SerializeField] private float forceMultiplier = 2;
    [SerializeField] private float velocityGainMultiplier = 1.5f;
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    public override void EntryAction()
    {
    }

    public override void ExitAction()
    {
        base.ExitAction();
        burstExplosionEffect.Play();
    }

    public override void Action()
    {
        float distanceFromExplosion = burstMaxDistance;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, burstMaxDistance, targetedLayers))
        {
            distanceFromExplosion = (cameraTransform.position - hit.point).magnitude;
        }

        // Only velocity that is causing the player to go slower will be added to the burst force speed
        Vector3 velocityFromDirection = Vector3.Project(playerRb.velocity, cameraTransform.forward);
        if (Mathf.Sign(Vector3.Dot(velocityFromDirection, cameraTransform.forward)) == 1)
        {
            playerRb.AddForce(-cameraTransform.forward * velocityFromDirection.magnitude * velocityGainMultiplier, ForceMode.VelocityChange);
        }

        playerRb.AddForce(-cameraTransform.forward * forceMultiplier / distanceFromExplosion, ForceMode.VelocityChange);
        Vector3 burstPosition = cameraTransform.position + cameraTransform.forward * distanceFromExplosion;
        playerSM.Play("Burst Explosion", burstPosition);
        burstExplosionEffectTransform.position = burstPosition;
        //Debug.DrawLine(playerRb.position, playerRb.position + direction, Color.red, 5);
    }
}