using UnityEngine;

[CreateAssetMenu(menuName = "Inputs/JumpBehaviorInputs", fileName = "JumpBehaviorInputs")]
public class JumpBehaviorInputs : ScriptableObject
{
    public float intialVelocity = 2f;
    public float holdingJumpForceMultiplier = 5f;
    public float jumpCurveOffset = 1f;
    public AnimationCurve jumpCurve;
}