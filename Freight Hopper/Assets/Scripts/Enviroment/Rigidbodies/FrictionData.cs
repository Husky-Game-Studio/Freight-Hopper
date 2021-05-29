using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Friction Data"), System.Serializable]
public class FrictionData : ScriptableObject
{
    public float Air => air;
    [SerializeField] private float air = 0.01f;
    public float Ground => ground;
    [SerializeField] private float ground = 0.08f;
}