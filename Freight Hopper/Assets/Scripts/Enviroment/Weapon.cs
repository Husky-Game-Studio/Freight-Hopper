using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private GameObject projectile;
    [SerializeField] private float speed = 1;
    [SerializeField] private float firerate = 1;
    [SerializeField] private float range = 30;
    [SerializeField] private Transform barrelEnd;
    [SerializeField] private Transform target;
    private bool readyToFire = true;

    private void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
        projectile = (GameObject)Resources.Load("Projectiles/Projectile");
    }

    private void Fire(Vector3 source, Vector3 target)
    {
        GameObject go = Instantiate(projectile, source, Quaternion.FromToRotation(source, target));
        Rigidbody rb = go.GetComponent<Rigidbody>();

        go.GetComponent<Projectile>().FaceTarget(target);
        rb.AddForce(this.transform.forward * speed, ForceMode.Impulse);
    }

    private void Update()
    {
        if (readyToFire && Vector3.Distance(this.transform.position, target.position) < range)
        {
            Fire(barrelEnd.position, target.position);
            StartCoroutine(Reload());
        }
        this.transform.LookAt(target);
    }

    private IEnumerator Reload()
    {
        readyToFire = false;
        yield return new WaitForSeconds(firerate);
        readyToFire = true;
    }
}