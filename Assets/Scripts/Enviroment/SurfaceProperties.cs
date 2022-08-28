using UnityEngine;

public class SurfaceProperties : MonoBehaviour
{
    // Player uses friction data for last surface touched, air friction matters
    [SerializeField] private FloatConst friction;
    public FloatConst Friction => friction;
}