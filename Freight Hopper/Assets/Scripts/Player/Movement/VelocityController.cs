using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VelocityController
{
    [SerializeField] private float speedLimit;
    [SerializeField] private float deltaAngle;
    [SerializeField] private float acceleration;
    [SerializeField, ReadOnly] private Vector3 velocity;
    [SerializeField, ReadOnly] private float speed;
    [SerializeField, ReadOnly] private Vector3 relativeHorizontalVelocity;
    [SerializeField, ReadOnly] private float relativeHorizontalSpeed;

    private float oppositeAngle;
    private PhysicsManager physicsManager;

    public void Initialize(PhysicsManager pm, float oppositeAngle)
    {
        this.physicsManager = pm;
        this.oppositeAngle = oppositeAngle;
    }

    public void RotateVelocity(Vector3 input)
    {
        physicsManager.rb.AddForce(-relativeHorizontalVelocity, ForceMode.VelocityChange);
        Vector3 rotatedVector = Vector3.RotateTowards(relativeHorizontalVelocity.normalized, input, deltaAngle, 0);
        physicsManager.rb.AddForce(rotatedVector * relativeHorizontalSpeed, ForceMode.VelocityChange);
    }

    public void Move(Vector3 input)
    {
        float nextSpeed = ((acceleration * Time.fixedDeltaTime * input) +
            relativeHorizontalVelocity).magnitude;

        if (nextSpeed < speedLimit || nextSpeed < relativeHorizontalSpeed)
        {
            physicsManager.rb.AddForce(input * acceleration, ForceMode.Acceleration);
        }
        else
        {
            RotateVelocity(input);
        }
        if (OppositeInput(input))
        {
            physicsManager.friction.ResetFrictionReduction();
        }
    }

    public void UpdateSpeedometer()
    {
        Vector3 velocity = physicsManager.rb.velocity;
        speed = velocity.magnitude;

        relativeHorizontalVelocity = velocity.ProjectOnContactPlane(physicsManager.collisionManager.ContactNormal.current)
            - physicsManager.rigidbodyLinker.ConnectionVelocity.current;
        relativeHorizontalSpeed = relativeHorizontalVelocity.magnitude;
    }

    public bool OppositeInput(Vector3 input)
    {
        if (input.IsZero())
        {
            return false;
        }
        Vector3 normalizedMomentumDirection = relativeHorizontalVelocity.normalized;
        return Vector3.Angle(normalizedMomentumDirection, input) > oppositeAngle;
    }
}