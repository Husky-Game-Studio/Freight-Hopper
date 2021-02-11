using UnityEngine;

[System.Serializable]
/// <summary>
/// PID controller, if you don't know what that is just google it. Going to be used for hovering, following a line, and more.
/// Heavily references: https://unitylist.com/p/103l/Unity-pid-with-rigidbodies
/// </summary>
public class PID
{
    /// <summary>
    /// The gain of the proportional error. This defines how much weight the proportional
    /// error has in the final output.
    /// </summary>
    [SerializeField] private float Kp;

    /// <summary>
    /// The gain of the integral error. This defines how much weight the integral
    /// error has in the final output.
    /// </summary>
    [SerializeField] private float Ki;

    /// <summary>
    /// The gain of the derivative error. This defines how much weight the derivative
    /// error has in the final output.
    /// </summary>
    [SerializeField] private float Kd;

    /// <summary>
    /// Tracks the current values of the proportional, integral, and derative errors.
    /// </summary>
    private float P, I, D;

    /// <summary>
    /// Used to keep track of what the error value was the last time the output was requested.
    /// This is used by the derivative to calculate the rate of change.
    /// </summary>
    private float previousError;

    public void Initialize(float p, float i, float d)
    {
        Kp = p;
        Ki = i;
        Kd = d;
    }

    /// <summary>
    /// Returns the amount of force that should be applied this frame
    /// to reach the target.
    /// </summary>
    /// <returns>
    /// The output of the PID calculation.
    /// </returns>
    /// <param name="currentError">How far away the body is from the target</param>
    /// <param name="deltaTime">The delta value from FixedUpdate.</param>
    public float GetOutput(float currentError, float deltaTime)
    {
        P = currentError;
        I += P * deltaTime;
        D = (P - previousError) / deltaTime;

        previousError = currentError;

        return P * Kp + I * Ki + D * Kd;
    }
}