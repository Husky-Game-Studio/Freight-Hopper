using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WindParticleController))]
public class Wind : MonoBehaviour
{
    [SerializeField] private bool active = true;
    private bool activated = false;
    [SerializeField] private float forcePerParticle = 1;
    private float rayWidth;

    // How many times per second wind status of rigidbodies are updated. Default 20, can cause performance issues at high numbers
    private readonly int updatesPerSecond = 20;

    [SerializeField] private Vector3 size;
    [SerializeField] private Vector3 offset;

    // Density of wind, higher densities means more wind particles per square unit. Can cause performance issues
    [SerializeField, Range(1.1f, 2.5f)] private float density = 1.65f;

    private List<Transform> windPossibleTargets = new List<Transform>();
    private Dictionary<Transform, ObjectScan> windTargets = new Dictionary<Transform, ObjectScan>();
    private Dictionary<Rigidbody, List<Ray>> affectedBodies = new Dictionary<Rigidbody, List<Ray>>();
    private Dictionary<Vector3Int, Ray> viableRays = new Dictionary<Vector3Int, Ray>();
    [SerializeField] private LayerMask affectedLayers;
    private WindParticleController windParticleController;

    private Vector3 Center => Vector3.forward * size.z / 2;

#if UNITY_EDITOR

    // Draws lines for wind direction
    private void OnDrawGizmosSelected()
    {
        rayWidth = 1 / density;
        Gizmos.color = Color.yellow;

        GizmosExtensions.DrawGizmosArrow(this.transform.position + offset, this.transform.forward);

        if (!active)
        {
            return;
        }
        for (float x = -1; x <= 1; x += rayWidth)
        {
            for (float y = -1; y <= 1; y += rayWidth)
            {
                Vector3 position = this.transform.TransformPoint(new Vector3(x, y, 0) + offset);
                Gizmos.DrawRay(position, this.transform.TransformDirection(Vector3.forward) * size.z);
            }
        }

        Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(offset + this.Center, size);
    }

#endif

    private void Awake()
    {
        windParticleController = this.GetComponent<WindParticleController>();

        rayWidth = 1 / density;
    }

    private void AddWindPossibleTargets()
    {
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();
        Portal[] portals = FindObjectsOfType<Portal>();
        for (int i = 0; i < portals.Length; i++)
        {
            windPossibleTargets.Add(portals[i].transform);
        }
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            windPossibleTargets.Add(rigidbodies[i].transform);
        }
        Player.PlayerLoadedIn += AddPlayerRigidbody;
    }

    private void AddPlayerRigidbody()
    {
        windPossibleTargets.Add(Player.Instance.transform);
    }

    private void OnEnable()
    {
        AddWindPossibleTargets();
    }

    private void OnDisable()
    {
        Player.PlayerLoadedIn -= AddPlayerRigidbody;
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
            UpdateWind();
        }
    }

    private void UpdateWind()
    {
        for (int i = 0; i < windPossibleTargets.Count; i++)
        {
            TryAddToScanner(windPossibleTargets[i]);
        }
        foreach (Rigidbody rb in affectedBodies.Keys)
        {
            affectedBodies[rb].Clear();
        }
        foreach (Transform transform in windTargets.Keys)
        {
            windTargets[transform].AddToScan(GetOrigin(transform));
            FindRigidbodies(transform);
        }
    }

    public Ray GetOrigin(Transform transform)
    {
        Plane plane = new Plane(this.transform.forward, this.transform.position);
        Vector3 back = plane.ClosestPointOnPlane(transform.position);
        return new Ray(back, this.transform.forward);
    }

    private Collider[] GetColliderInformation(Transform transform)
    {
        int arraySize = 0;
        Collider onObject = transform.GetComponent<Collider>();
        if (onObject != null)
        {
            arraySize++;
        }

        Collider[] colliders = transform.GetComponentsInChildren<Collider>();
        arraySize += colliders.Length;
        Collider[] allColliders = new Collider[arraySize];
        int j = 0;
        for (int i = 0; i < allColliders.Length; i++)
        {
            if (onObject != null && i == 0)
            {
                allColliders[0] = onObject;
                continue;
            }
            allColliders[i] = colliders[j];
            j++;
        }
        return allColliders;
    }

    private void TryAddToScanner(Transform transform)
    {
        if (PositionInsideWind(transform.position) && !windTargets.ContainsKey(transform))
        {
            windTargets.Add(transform,
                new ObjectScan(GetColliderInformation(transform), rayWidth, viableRays,
                this.transform, size.z, PositionInsideWind));
        }
        else if (!PositionInsideWind(transform.position) && windTargets.ContainsKey(transform))
        {
            windTargets.Remove(transform);
        }
    }

    private bool PositionInsideWind(Vector3 position)
    {
        return GravityZone.IsPointInBoxRegion(this.transform, this.Center + offset, size / 2, position);
    }

    private void FindRigidbodies(Transform transformKey)
    {
        foreach (Ray ray in viableRays.Values)
        {
            SendRay(ray, size.z);
        }
        viableRays.Clear();
    }

    private void WindPortalTeleportation(RaycastHit hit, float distance, Ray ray)
    {
        Portal portal = hit.collider.gameObject.GetComponent<Portal>();
        if (portal != null)
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue);
            float distanceLeft = distance - Vector3.Distance(hit.point, ray.origin);
            portal.TeleportRay(ref ray, hit.point);
            Vector3 portalSize = portal.OtherPortal().GetComponent<BoxCollider>().size;

            WindParticleController windParticleController = portal.OtherPortal().GetComponent<WindParticleController>();
            if (portal.OtherPortal().GetComponent<WindParticleController>() == null)
            {
                windParticleController = portal.OtherPortal().gameObject.AddComponent<WindParticleController>();
            }

            SendRay(ray, distanceLeft);
            windParticleController.SpawnParticleSystem(Vector3.zero, new Vector3(portalSize.x, portalSize.y, distanceLeft), ray.direction, portal.OtherPortal());
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
        }
    }

    private void WindRigidbodyFind(RaycastHit hit, float distance, Ray ray)
    {
        Rigidbody colliderRb = hit.collider.attachedRigidbody;
        if (colliderRb != null)
        {
            if (!affectedBodies.ContainsKey(colliderRb))
            {
                affectedBodies.Add(colliderRb, new List<Ray>());
            }
            affectedBodies[colliderRb].Add(new Ray(hit.point, ray.direction));

            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
        }
    }

    private void SendRay(Ray ray, float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance, affectedLayers))
        {
            if (hit.collider.isTrigger)
            {
                WindPortalTeleportation(hit, distance, ray);
            }
            else
            {
                WindRigidbodyFind(hit, distance, ray);
            }
        }
    }

    private void ApplyWind()
    {
        foreach (Rigidbody rb in affectedBodies.Keys)
        {
            foreach (Ray ray in affectedBodies[rb])
            {
                rb.AddForceAtPosition(ray.direction * forcePerParticle, ray.origin, ForceMode.Force);
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
            ApplyWind();
        }
    }
}