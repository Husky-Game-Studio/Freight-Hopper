using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private float forcePerPosition = 1;
    [SerializeField] private int updatesPerSecond = 5;
    [SerializeField] private Vector3 size;

    [SerializeField, Range(0.4f, 0.9f)] private float width = 1;
    [SerializeField] private Dictionary<Rigidbody, List<Ray>> affectedBodies = new Dictionary<Rigidbody, List<Ray>>();
    [SerializeField] private LayerMask layers;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        GizmosExtensions.DrawGizmosArrow(this.transform.position, this.transform.forward);

        for (float x = -size.x / 2; x <= size.x / 2; x += width)
        {
            for (float y = -size.y / 2; y <= size.y / 2; y += width)
            {
                Vector3 position = transform.TransformPoint(new Vector3(x, y, 0));
                Gizmos.DrawRay(position, transform.TransformDirection(Vector3.forward) * size.z);
            }
        }
    }

    private void Awake()
    {
        StartCoroutine(WindLoop(updatesPerSecond));
    }

    private IEnumerator WindLoop(float frequency)
    {
        while (true)
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
        for (float x = -windSize.x / 2; x <= windSize.x / 2; x += width)
        {
            for (float y = -windSize.y / 2; y <= windSize.y / 2; y += width)
            {
                Vector3 position = source.transform.TransformPoint(new Vector3(x, y, 0));
                Ray ray = new Ray(position, direction);
                SendRay(ref ray, out hit, windSize.z);
            }
        }
    }

    private void SendRay(ref Ray ray, out RaycastHit hit, float distance)
    {
        if (Physics.Raycast(ray, out hit, distance, layers))
        {
            if (hit.collider.isTrigger)
            {
                Portal portal = hit.collider.gameObject.GetComponent<Portal>();
                if (portal != null)
                {
                    float distanceLeft = distance - Vector3.Distance(hit.point, ray.origin);
                    portal.TeleportRay(ref ray, hit.point);
                    SendRay(ref ray, out hit, distanceLeft);

                    //Debug.DrawRay(ray.origin, ray.direction * distanceLeft, Color.blue);
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
                    //Debug.Log("Hitting " + hit.transform.name);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (Rigidbody rb in affectedBodies.Keys)
        {
            foreach (Ray ray in affectedBodies[rb])
            {
                rb.AddForceAtPosition(ray.direction * forcePerPosition, ray.origin, ForceMode.Force);
            }
        }
    }
}