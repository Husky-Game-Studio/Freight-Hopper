using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Rigidbody myRB;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 4f);
        myRB = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //myRB.AddForce(0, 0, 100f);
    }
}
