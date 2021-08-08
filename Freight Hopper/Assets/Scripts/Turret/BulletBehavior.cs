using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Rigidbody myRB;
    public float projectileForce = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 4f);
        myRB = this.gameObject.GetComponent<Rigidbody>();
        myRB.AddForce(this.gameObject.transform.forward.normalized * projectileForce, ForceMode.Impulse);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<OnTriggerEvent>() == null)
        {
            Debug.Log("DestroyBullet");
            Destroy(this.gameObject, 0.05f);
        }
    }
}
