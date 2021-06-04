using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceProperties : MonoBehaviour
{
    // Player uses friction data for last surface touched, air friction matters
    [SerializeField] private FrictionData friction;

    public FrictionData Friction => friction;
}