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

    private bool releasedJump;
    [SerializeField, ReadOnly] private bool doubleJumpReady;

    private void Awake()
    {
        playerCollision = GetComponent<CollisionCheck>();
        jumpBehavior = GetComponent<JumpBehavior>();
    }

    private void OnEnable()
    {
        UserInput.JumpInput += TryDoubleJump;
        playerCollision.Landed += DoubleJumpUsed;
    }

    private void OnDisable()
    {
        UserInput.JumpInput -= TryDoubleJump;
        playerCollision.Landed -= DoubleJumpUsed;
    }

    private void Update()
    {
        if (!playerCollision.IsGrounded && !UserInput.Input.Jump())
        {
            releasedJump = true;
        }
    }

    private void DoubleJumpUsed()
    {
        doubleJumpReady = true;
    }

    private void TryDoubleJump()
    {
        if (!playerCollision.IsGrounded && releasedJump && doubleJumpReady && !jumpBehavior.CanJump)
        {
            releasedJump = false;
            doubleJumpReady = false;
            jumpBehavior.Jump(jumpBehavior.JumpHeight * (percentStrengthComparedToNormalJump / 100));
        }
    }
}