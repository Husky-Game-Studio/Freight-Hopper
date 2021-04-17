using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WindParticleController))]
public class Wind : MonoBehaviour
{
    [SerializeField] private bool active = true;
    private bool activated = false;
    [SerializeField] private float forcePerParticle = 1;

    // How many times per second wind status of rigidbodies are updated. Default 20, can cause performance issues at high numbers
    private readonly int updatesPerSecond = 20;

    [SerializeField] private Vector3 size;
    [SerializeField] private Vector3 offset;

    // Density of wind, higher densities means more wind particles per square unit. Can cause performance issues
    [SerializeField, Range(4f, 9f)] private float density = 6f;

    private Dictionary<Rigidbody, List<Ray>> affectedBodies = new Dictionary<Rigidbody, List<Ray>>();
    [SerializeField] private LayerMask affectedLayers;
    private WindParticleController windParticleController;

    // Draws lines for wind direction
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        GizmosExtensions.DrawGizmosArrow(this.transform.position + offset, this.transform.forward);

        for (float x = -size.x / 2; x <= size.x / 2; x += density / 10)
        {
            for (float y = -size.y / 2; y <= size.y / 2; y += density / 10)
            {
                Vector3 position = transform.TransformPoint(new Vector3(x, y, 0) + offset);
                Gizmos.DrawRay(position, transform.TransformDirection(Vector3.forward) * size.z);
            }
        }
    }

    private void Awake()
    {
        windParticleController = this.GetComponent<WindParticleController>();
    }

    public void Activate()
    {
        if (active && !activated)
        {
            activated = true;
            StartCoroutine(WindLoop(updatesPerSecond));
            windParticleController.SpawnParticleSystem(offset, size, this.transform.forward, this.transform);
        }
    }

    public void Deactivate()
    {
        if (activated)
        {
            activated = false;
            active = false;
            StopCoroutine(WindLoop(updatesPerSecond));
            windParticleController.DisableParticles();
        }
    }

    private IEnumerator WindLoop(float frequency)
    {
        while (active)
        {
            yield return new WaitForSecondsRealtime(1 / (float)frequency);
            FindRigidbodies(size, this.transform);
        }
    }

    private void FindRigidbodies(Vector3 windSize, Transform source)
    {
        foreach (Rigidbody rb in affectedBodies.Keys)
        {
            affectedBodies[rb].Clear();
        }

        RaycastHit hit;
        Vector3 direction = source.transform.forward;
        for (float x = -windSize.x / 2; x <= windSize.x / 2; x += density / 10)
        {
            for (float y = -windSize.y / 2; y <= windSize.y / 2; y += density / 10)
            {
                Vector3 position = source.transform.TransformPoint(new Vector3(x, y, 0) + offset);
                Ray ray = new Ray(position, direction);
                SendRay(ref ray, out hit, windSize.z);
            }
        }
    }

    private void SendRay(ref Ray ray, out RaycastHit hit, float distance)
    {
        if (Physics.Raycast(ray, out hit, distance, affectedLayers))
        {
            if (hit.collider.isTrigger)
            {
                // Wind portal teleportation
                Portal portal = hit.collider.gameObject.GetComponent<Portal>();
                if (portal != null)
                {
                    float distanceLeft = distance - Vector3.Distance(hit.point, ray.origin);
                    portal.TeleportRay(ref ray, hit.point);
                    Vector3 portalSize = portal.OtherPortal().GetComponent<BoxCollider>().size;

                    WindParticleController windParticleController = portal.OtherPortal().GetComponent<WindParticleController>();
                    if (portal.OtherPortal().GetComponent<WindParticleController>() == null)
                    {
                        windParticleController = portal.OtherPortal().gameObject.AddComponent<WindParticleController>();
                    }

                    SendRay(ref ray, out hit, distanceLeft);
                    windParticleController.SpawnParticleSystem(Vector3.zero, new Vector3(portalSize.x, portalSize.y, distanceLeft), ray.direction, portal.OtherPortal());
                }
            }
            else
            {
                if (hit.collider.attachedRigidbody != null)
                {
                    if (!affectedBodies.ContainsKey(hit.collider.attachedRigidbody))
                    {
                        affectedBodies.Add(hit.collider.attachedRigidbody, new List<Ray>());
                    }
                    affectedBodies[hit.collider.attachedRigidbody].Add(new Ray(hit.point, ray.direction));
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (active && !activated)
        {
            Activate();
        }
        if (active && activated)
        {
            windParticleController.UpdateSettings(size, this.transform.forward);
            windParticleController.EnableParticles();
        }
        if (!active && activated)
        {
            Deactivate();
        }
        if (activated)
        {
            foreach (Rigidbody rb in affectedBodies.Keys)
            {
                foreach (Ray ray in affectedBodies[rb])
                {
                    rb.AddForceAtPosition(ray.direction * forcePerParticle, ray.origin, ForceMode.Force);
                }
            }
        }
    }
}