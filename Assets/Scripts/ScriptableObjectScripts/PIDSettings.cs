using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PID Settings"), System.Serializable]
public class PIDSettings : ScriptableObject
{
    public float Kp;

    public float Ki;

    public float Kd;

    // Scales Kp, Ki, Kd by amount
    public float scalar;
}