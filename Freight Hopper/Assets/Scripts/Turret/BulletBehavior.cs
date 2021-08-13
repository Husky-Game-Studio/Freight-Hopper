using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

// Help from: https://www.youtube.com/watch?v=IvT8hjy6q4o
public class BulletBehavior : MonoBehaviour
{
    /*private Rigidbody myRB;
    private float projectileForce = 0f;*/
    private Vector3 targetPosition = Vector3.zero;
    
    // maximumVerticleDisplacement must be less than the gravity to work
    /*private float maximumVerticleDisplacement = 100f;
    private float gravity;

    private bool debugPath = true;*/
    
    /*void Start()
    {
        //Destroy(this.gameObject, 4f);
        myRB = this.gameObject.GetComponent<Rigidbody>();
        //myRB.AddForce(this.gameObject.transform.forward.normalized * projectileForce, ForceMode.Impulse);
        gravity = Physics.gravity.y;
        Launch();
    }

    private void Update()
    {
        if (debugPath)
        {
            DrawPath();
        }
    }*/

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<OnTriggerEvent>() == null)
        {
            Debug.Log("DestroyBullet");
            Destroy(this.gameObject, 0.05f);
        }
    }

    /*private void Launch()
    {
        myRB.velocity = CalculateLaunchData ().initialVeloctiy;
    }
    
    private LaunchData CalculateLaunchData()
    {
        float displacementY = targetPosition.y - myRB.position.y;
        Vector3 displacementXZ =
            new Vector3(targetPosition.x - myRB.position.x, 0, targetPosition.z - myRB.position.z);
        float time =
            (Mathf.Sqrt(-2 * gravity) + Mathf.Sqrt(2 * (displacementY - maximumVerticleDisplacement) / gravity));
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * maximumVerticleDisplacement);
        Vector3 velocityXZ = displacementXZ / time;
        
        
        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    private void DrawPath()
    {
        LaunchData launchData = CalculateLaunchData();
        Vector3 previousDrawPoint = myRB.position;
        
        
        int resolution = 30;
        for (int i = 0; i < resolution; i++)
        {
            float simulationTime = i / (float) resolution * launchData.timeToTarget;
            Vector3 displacement = (launchData.initialVeloctiy * simulationTime) + (Vector3.up *
                                   gravity) * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = myRB.position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }
    }

    struct LaunchData
    {
        public readonly Vector3 initialVeloctiy;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVeloctiy, float timeToTarget)
        {
            this.initialVeloctiy = initialVeloctiy;
            this.timeToTarget = timeToTarget;
        }
        
    }
    
    public void SetTargetPosition(Vector3 position)
     {
         targetPosition = position;
     }
     
     public void SetProjectileForce(float force)
     {
         projectileForce = force;
     }*/
}
