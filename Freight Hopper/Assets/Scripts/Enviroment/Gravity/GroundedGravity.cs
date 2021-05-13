using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionManagement))]
public class GroundedGravity : Gravity
{
    private CollisionManagement collision;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        collision = GetComponent<CollisionManagement>();
    }

    public void OnEnable()
    {
        collision.CollisionDataCollected += GravityLoop;
    }

    public void OnDisable()
    {
        collision.CollisionDataCollected -= GravityLoop;
    }

    protected override void GravityLoop()
    {
        if (!useGravity || collision.IsGrounded.current)
        {
            return;
        }
        ApplyGravityForce();
    }
}