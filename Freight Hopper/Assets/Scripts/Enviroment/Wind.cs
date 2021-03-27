using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private float forcePerPosition = 1;
    [SerializeField] private int updatesPerSecond = 5;
    [SerializeField] private Vector3 size;

    [SerializeField, Range(0.4f, 0.9f)] private float width = 1;
    [SerializeField] private Dictionary<Rigidbody, List<Vector3>> affectedBodies = new Dictionary<Rigidbody, List<Vector3>>();
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
            FindRigidbodies();
        }
    }

    private void FindRigidbodies()
    {
        foreach (Rigidbody rb in affectedBodies.Keys)
        {
            affectedBodies[rb].Clear();
        }

        RaycastHit hit;
        for (float x = -size.x / 2; x <= size.x / 2; x += width)
        {
            for (float y = -size.y / 2; y <= size.y / 2; y += width)
            {
                Vector3 position = transform.TransformPoint(new Vector3(x, y, 0));

                if (Physics.Raycast(position, transform.forward, out hit, size.z, layers))
                {
                    if (hit.collider.attachedRigidbody != null)
                    {
                        if (!affectedBodies.ContainsKey(hit.collider.attachedRigidbody))
                        {
                            affectedBodies.Add(hit.collider.attachedRigidbody, new List<Vector3>());
                        }
                        affectedBodies[hit.collider.attachedRigidbody].Add(hit.point);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (Rigidbody rb in affectedBodies.Keys)
        {
            foreach (Vector3 position in affectedBodies[rb])
            {
                rb.AddForceAtPosition(transform.forward * forcePerPosition, position, ForceMode.Force);
            }
        }
    }
}