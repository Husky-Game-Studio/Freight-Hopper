using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float despawnTime = 5;

    public void FaceTarget(Vector3 target)
    {
        this.transform.LookAt(target);
        Destroy(this.gameObject, despawnTime);
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        Instantiate(Resources.Load("Projectiles/Explosion"), this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}