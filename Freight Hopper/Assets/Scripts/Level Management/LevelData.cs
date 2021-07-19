using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public enum NextLevelStatus
    {
        NextLevel,
        NextWorld,
        Menu,
        Credits,
    }

    public NextLevelStatus nextLevelStatus = NextLevelStatus.NextLevel;
    public Vector3 spawnPosition = Vector3.zero;
    public float rotationAngle = 0;
    public Quaternion velocityDirection;
    public float speed;

    [SerializeField]
    public PlayerAbilities.Name[] activeAbilities = {
            PlayerAbilities.Name.MovementBehavior,
            PlayerAbilities.Name.JumpBehavior,
            PlayerAbilities.Name.WallRunBehavior,
            PlayerAbilities.Name.GroundPoundBehavior
        };
}