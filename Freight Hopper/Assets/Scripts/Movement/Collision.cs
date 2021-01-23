using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public void Respawn()
    {
        this.transform.position = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelSettings>().SpawnPosition;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (!collision.collider.CompareTag("landable"))
        {
            Respawn();
        }
        else
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                Camera.main.GetComponent<CameraDrag>().CollidDrag(-1 * collision.GetContact(i).normal);
            }
        }
    }
}