using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [SerializeField] private GameObject projectile;
    [SerializeField, Tooltip("-1 is infinite"), Min(-1)] private int ammoCount;

    public override void Fire(Vector3 source, Vector3 forward, Vector3 target)
    {
        if (!canFire)
        {
            return;
        }
        base.Fire(source, forward, target);

        GameObject bullet = Instantiate(projectile, source, Quaternion.identity);
        bullet.transform.forward = forward;
        bullet.GetComponent<Rigidbody>().velocity = forward * projectileSpeed;
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        //Instantiate(Resources.Load("Projectiles/Explosion"), this.transform.position, Quaternion.identity);
        //Destroy(this.gameObject);
    }
}