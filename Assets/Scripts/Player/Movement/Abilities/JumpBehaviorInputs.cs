using UnityEngine;

[CreateAssetMenu(menuName = "Inputs/JumpBehaviorInputs", fileName = "JumpBehaviorInputs")]
public class JumpBehaviorInputs : ScriptableObject
{
    public float intialVelocity = 2f;
    public AnimationCurve jumpCurve;
    public AnimationCurve gravityRise;
    public float upwardBiasWeight = 1;
}