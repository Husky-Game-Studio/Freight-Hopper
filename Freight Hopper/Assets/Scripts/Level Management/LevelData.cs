using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public enum NextLevelStatus
    {
        NextLevel,
        NextWorld,
        Menu,
        Credits,
    }

    [SerializeField] public NextLevelStatus nextLevelStatus;
    [SerializeField] public Vector3 spawnPosition = Vector3.zero;
}