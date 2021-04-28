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

    [SerializeField] public NextLevelStatus nextLevelStatus = NextLevelStatus.NextLevel;
    [SerializeField] public Vector3 spawnPosition = Vector3.zero;

    [SerializeField]
    public PlayerAbilities.Name[] activeAbilities = {
            PlayerAbilities.Name.MovementBehavior,
            PlayerAbilities.Name.JumpBehavior,
            PlayerAbilities.Name.WallRunBehavior,
            PlayerAbilities.Name.GroundPoundBehavior
        };
}