using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoTransform : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    Vector3 posTarget;
    [SerializeField]
    Vector3 velTarget;

    [SerializeField] [ReadOnly]
    private float rotCurrent;
    [SerializeField] [ReadOnly]
    private Vector3 newPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //Forward, upward and roational have a range they individually abide by
    void PowerMovement(float forward, float rotational, Vector3 moveTemp)
    {
        //rb.AddRelativeForce(Vector3.forward * forward);
        this.gameObject.transform.position += moveTemp;
        rb.AddRelativeTorque(Vector3.up * rotational);

    }

    void FixedUpdate()
    {
        Vector3 posCurrent = rb.position;
        Vector3 posDelta = posTarget - posCurrent;
        rotCurrent = this.transform.eulerAngles.y * Mathf.Deg2Rad;
        Matrix4x4 mat = new Matrix4x4(
            new Vector4(Mathf.Cos(rotCurrent), 0, Mathf.Sin(rotCurrent), 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(-Mathf.Sin(rotCurrent), 0, Mathf.Cos(rotCurrent), 0),
            new Vector4(0, 0, 0, 1));
        newPos = mat * posDelta;
        newPos = newPos.normalized;

        Vector3 velCurrent = rb.velocity;

        float rotVelTarget = Mathf.Atan2(velTarget.z, velTarget.x);
        float velMagTarget = velTarget.magnitude;
        float rotVelCurrent = Mathf.Atan2(rb.velocity.z, rb.velocity.x);
        float velMagCurrent = rb.velocity.magnitude;

        PowerMovement(newPos.z * 0.05f, newPos.x * 4.0f, newPos.z * 0.05f * (mat.inverse * Vector3.forward));
        //Vector3.Dot(new Vector3(Mathf.Sin(rotCurrent), 0, Mathf.Cos(rotCurrent)), newPos.normalized) * (velMagTarget - velMagCurrent)
        Debug.DrawLine(posCurrent, posTarget);
        Debug.DrawLine(posCurrent, posCurrent + (10.0f* (Vector3)(mat.inverse * Vector3.forward)));

    }
}
