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
        Rigidbody hitRigidbody = null;
        float distanceFromExplosion = burstMaxDistance;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, burstMaxDistance, targetedLayers))
        {
            distanceFromExplosion = (cameraTransform.position - hit.point).magnitude;

            hitRigidbody = hit.rigidbody;
        }

        if (distanceFromExplosion < .6f)
        {
            playerSM.Play("FastBurst");
        }

        // Only velocity that is causing the player to go slower will be added to the burst force speed
        Vector3 velocityFromDirection = Vector3.Project(playerPM.rb.velocity, cameraTransform.forward);
        if (Mathf.Sign(Vector3.Dot(velocityFromDirection, cameraTransform.forward)) == 1)
        {
            Vector3 forceV = -cameraTransform.forward * velocityFromDirection.magnitude * velocityGainMultiplier;
            playerPM.rb.AddForce(forceV, ForceMode.VelocityChange);
            if (hitRigidbody != null)
            {
                hitRigidbody.AddForce(-forceV * playerPM.rb.mass, ForceMode.Impulse);
            }
        }
        Vector3 force = -cameraTransform.forward * forceMultiplier / distanceFromExplosion;
        playerPM.rb.AddForce(force, ForceMode.VelocityChange);
        if (hitRigidbody != null)
        {
            hitRigidbody.AddForce(-force * playerPM.rb.mass, ForceMode.Impulse);
        }

        Vector3 burstPosition = cameraTransform.position + cameraTransform.forward * distanceFromExplosion;
        playerSM.Play("BurstExplosion", burstPosition);
        burstExplosionEffectTransform.position = burstPosition;
        //Debug.DrawLine(playerRb.position, playerRb.position + direction, Color.red, 5);
    }
}