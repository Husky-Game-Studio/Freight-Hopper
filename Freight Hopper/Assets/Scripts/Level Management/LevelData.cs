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
        Custom
    }

    public NextLevelStatus nextLevelStatus = NextLevelStatus.NextLevel;
    public string customNextLevelName;
    public Vector3 spawnPosition = Vector3.zero;
    public float rotationAngle = 0;
    public Quaternion velocityDirection;
    public float speed;
    public float[] medalTimes = new float[4];

    [SerializeField]
    public PlayerAbilities.Name[] activeAbilities = {
            PlayerAbilities.Name.MovementBehavior,
            PlayerAbilities.Name.JumpBehavior,
            PlayerAbilities.Name.WallRunBehavior,
            PlayerAbilities.Name.GroundPoundBehavior
        };

    public void SetSpawnTransform(Transform transform)
    {
        spawnPosition = transform.position;
        rotationAngle = Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up);
    }
}