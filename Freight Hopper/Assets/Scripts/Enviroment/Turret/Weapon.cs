using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField, Tooltip("1 meaning 1 per second"), Min(0)] protected float firerate = 1;

    [SerializeField, ReadOnly] protected bool canFire = true;
    [SerializeField, Min(0)] protected float projectileSpeed;
    public float ProjectileSpeed => projectileSpeed;
    public bool CanFire => canFire;

    public virtual void Fire(Vector3 source, Vector3 forward, Vector3 targetPosition)
    {
        if (!canFire)
        {
            return;
        }
        StartCoroutine(Reload());
    }

    public virtual IEnumerator Reload()
    {
        canFire = false;
        yield return new WaitForSeconds(firerate);
        canFire = true;
    }
}