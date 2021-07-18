using UnityEngine;

[System.Serializable]
public class VelocityController
{
    [SerializeField] private float speedLimit;
    [SerializeField] private float deltaAngle;
    [SerializeField] private float acceleration;

    private float oppositeAngle;
    private PhysicsManager physicsManager;
    private Speedometer speedometer;

    public void Initialize(PhysicsManager pm, Speedometer speedometer, float oppositeAngle)
    {
        this.physicsManager = pm;
        this.speedometer = speedometer;
        this.oppositeAngle = oppositeAngle;
    }

    public void RotateVelocity(Vector3 input)
    {
        physicsManager.rb.AddForce(-speedometer.HorzVelocity, ForceMode.VelocityChange);
        Vector3 rotatedVector = Vector3.RotateTowards(speedometer.HorzVelocity.normalized, input, deltaAngle, 0);
        physicsManager.rb.AddForce(rotatedVector * speedometer.HorzSpeed, ForceMode.VelocityChange);
    }

    public void Move(Vector3 input)
    {
        float nextSpeed = ((acceleration * Time.fixedDeltaTime * input) +
            speedometer.HorzVelocity).magnitude;

        if (nextSpeed < speedLimit || nextSpeed < speedometer.HorzSpeed)
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

    public bool OppositeInput(Vector3 input)
    {
        if (input.IsZero())
        {
            return false;
        }
        Vector3 normalizedMomentumDirection = speedometer.HorzVelocity.normalized;
        return Vector3.Angle(normalizedMomentumDirection, input) > oppositeAngle;
    }
}