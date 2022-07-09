using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Accelerometer : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 velF = Vector3.zero;
    private Vector3 velI = Vector3.zero;
    [SerializeField] [ReadOnly]
    private Vector3 acc;

    private Vector3 angVelF = Vector3.zero;
    private Vector3 angVelI = Vector3.zero;
    [SerializeField] [ReadOnly]
    private Vector3 angAcc;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    /*
    private void FixedUpdate()
    {
        //Translational Velocity
        velF = rb.velocity;
        acc = (velF - velI) / Time.fixedDeltaTime;
        velI = rb.velocity;
        //Angular Velocity
        angVelF = rb.angularVelocity;
        angAcc = (angVelF - angVelI) / Time.fixedDeltaTime;
        angVelI = rb.angularVelocity;
    }
    */

    public Vector3 GetAcceleration()
    {
        var velocity = rb.velocity;
        velF = velocity;
        acc = (velF - velI) / Time.fixedDeltaTime;
        velI = velocity;
        return acc;
    }

    public Vector3 GetAngularAcceleration()
    {
        var angularVelocity = rb.angularVelocity;
        angVelF = angularVelocity;
        angAcc = (angVelF - angVelI) / Time.fixedDeltaTime;
        angVelI = angularVelocity;
        return angAcc;
    }
}
