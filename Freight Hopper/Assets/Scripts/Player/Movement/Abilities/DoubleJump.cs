using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheck), typeof(JumpBehavior))]
public class DoubleJump : MonoBehaviour
{
    private CollisionCheck playerCollision;
    private JumpBehavior jumpBehavior;

    [Range(25, 300)]
    [SerializeField] private float percentStrengthComparedToNormalJump;

    [SerializeField, ReadOnly] private bool doubleJumpReady;

    private void Awake()
    {
        playerCollision = GetComponent<CollisionCheck>();
        jumpBehavior = GetComponent<JumpBehavior>();
    }

    private void OnEnable()
    {
        UserInput.JumpInput += TryDoubleJump;
        playerCollision.Landed += Landed;
    }

    private void OnDisable()
    {
        UserInput.JumpInput -= TryDoubleJump;
        playerCollision.Landed -= Landed;
    }

    private void Landed()
    {
        doubleJumpReady = true;
    }

    private void TryDoubleJump()
    {
        if (!playerCollision.IsGrounded.current && doubleJumpReady && !jumpBehavior.CanJump)
        {
            doubleJumpReady = false;
            jumpBehavior.Jump(jumpBehavior.JumpHeight * (percentStrengthComparedToNormalJump / 100));
        }
    }
}