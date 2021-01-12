using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private float strength = 1;
    [SerializeField] private List<Rigidbody> effectedBodies = new List<Rigidbody>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * strength);
    }

    private void Awake()
    {
        // This is not a good way to find the player, but idk how to find the player since its in another scene.
        // At least not yet
    }

    private void Start()
    {
        effectedBodies.Add(UserInput.Input.gameObject.GetComponent<Rigidbody>());
    }

    private void FixedUpdate()
    {
        foreach (Rigidbody rb in effectedBodies)
        {
            rb.AddForce(transform.TransformDirection(Vector3.forward) * strength, ForceMode.Force);
        }
    }
}