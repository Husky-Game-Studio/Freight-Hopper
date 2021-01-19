using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public static void Respawn(Transform trans, Rigidbody rb)
    {
        trans.position = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelSettings>().SpawnPosition;
        rb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (!collision.collider.CompareTag("landable"))
        {
            Respawn(this.transform, GetComponent<Rigidbody>());
        }
    }
}