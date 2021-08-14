using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

// Help from: https://www.youtube.com/watch?v=IvT8hjy6q4o
public class BulletBehavior : MonoBehaviour
{
    private Vector3 currentDirection;
    private Vector3 previousDirection;
    
    void Start()
    {
        Destroy(this.gameObject, 4f);
    }

    private void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<OnTriggerEvent>() == null)
        {
            Destroy(this.gameObject, 0.05f);
        }
    }

}
