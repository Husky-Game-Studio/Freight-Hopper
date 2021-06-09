using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Generic/Float Const"), System.Serializable]
public class FloatConst : ScriptableObject
{
    public float Value => value;
    [SerializeField] private float value;
}