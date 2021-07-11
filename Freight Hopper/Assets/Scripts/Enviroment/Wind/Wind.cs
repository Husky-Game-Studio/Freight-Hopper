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
    [SerializeField] private LayerMask affectedLayers;
    [SerializeField] private LayerMask targetedLayers;
    private WindParticleController windParticleController;

    private Vector3 Center => this.transform.forward * size.z / 2;

#if UNITY_EDITOR

    // Draws lines for wind direction
    private void OnDrawGizmosSelected()
    {
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

        Gizmos.matrix = Matrix4x4.TRS(this.transform.position + this.Center, this.transform.rotation, Vector3.one);

        Gizmos.DrawWireCube(offset, size);
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

    private void OnValidate()
    {
        rayWidth = 1 / density;
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

    public Ray GetOrigin(Transform transform)
    {
        Vector3 back = Vector3.ProjectOnPlane(transform.position, this.transform.position) - (this.transform.forward * size.z / 2);

        return new Ray(back, this.transform.forward);
    }

    private void TryAddToScanner(Transform transform)
    {
        if (PositionInsideWind(transform.position) && !windTargets.ContainsKey(transform))
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

            windTargets.Add(transform, new ObjectScan(transform, allColliders, rayWidth, GetOrigin(transform), this.transform, size.z, PositionInsideWind));
            windTargets[transform].CreateScan();
        }
        else if (!PositionInsideWind(transform.position) && windTargets.ContainsKey(transform))
        {
            windTargets.Remove(transform);
        }
    }

    private bool PositionInsideWind(Vector3 position)
    {
        return GravityZone.IsPointInBoxRegion(this.transform, this.Center, size, position);
    }

    private void FindRigidbodies(Vector3 windSize, Transform source)
    {
        foreach (Rigidbody rb in affectedBodies.Keys)
        {
            affectedBodies[rb].Clear();
        }

        Vector3 direction = source.transform.forward;
        for (float x = -windSize.x / 2; x <= windSize.x / 2; x += rayWidth)
        {
            for (float y = -windSize.y / 2; y <= windSize.y / 2; y += rayWidth)
            {
                Vector3 position = source.transform.TransformPoint(new Vector3(x, y, 0) + offset);
                Ray ray = new Ray(position, direction);
                SendRay(ref ray, out _, windSize.z);
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
                // Wind on a rigidbody
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
            for (int i = 0; i < windPossibleTargets.Count; i++)
            {
                TryAddToScanner(windPossibleTargets[i]);
            }
            /*foreach (Transform transform in windTargets.Keys)
            {
                //Ray rayOrigin = GetOrigin(transform);
                //windTargets[transform].UpdateOrigin(rayOrigin);
                //Debug.Log("transform position " + transform.position + " ray origin " + rayOrigin.origin);
                //windTargets[transform].CreateScan();
                //windTargets[transform].ShowRays();
            }*/
            ApplyWind();
        }
    }
}