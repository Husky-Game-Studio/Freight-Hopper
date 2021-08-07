using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Rigidbody myRB;
    public float projectileForce;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 4f);
        myRB = this.gameObject.GetComponent<Rigidbody>();
        myRB.AddForce(this.gameObject.transform.forward.normalized * projectileForce, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        //myRB.AddForce(0, 0, 100f);
        
    }
}
