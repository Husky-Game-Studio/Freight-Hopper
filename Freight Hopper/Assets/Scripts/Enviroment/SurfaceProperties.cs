using UnityEngine;

public class SurfaceProperties : MonoBehaviour
{
    // if not landable, the surface will kill if player is touching
    [SerializeField] private bool killInstantly = true;

    // Player uses friction data for last surface touched, air friction matters
    [SerializeField] private FloatConst friction;
    public FloatConst Friction => friction;
    public bool KillInstantly => killInstantly;
}