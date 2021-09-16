using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakTrainJointOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb == null)
        {
            return;
        }
        CartProperties cartProperties = rb.GetComponent<CartProperties>();
        if (collision.rigidbody.GetComponent<CartProperties>())
        {
            cartProperties.SnapJoint();
        }
    }
}