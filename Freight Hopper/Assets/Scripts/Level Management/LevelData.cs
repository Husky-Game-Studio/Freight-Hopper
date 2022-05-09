using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/Level/Data"), System.Serializable]
public class LevelData : ScriptableObject
{
    public enum NextLevelStatus
    {
        NextLevel,
        NextWorld,
        Menu,
        Credits,
        Custom
    }

    [SerializeField] private NextLevelStatus nextLevelStatus = NextLevelStatus.NextLevel;
    [SerializeField] private string customNextLevelName;
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;
    [SerializeField] private float rotationAngle = 0;
    [SerializeField] private Quaternion velocityDirection;
    [SerializeField] private float speed;
    [SerializeField] private bool snapDownAtStart = true;
    [SerializeField] private LayerMask layerMask;

    [Header("Meta Data")]
    [SerializeField] private string title;
    [SerializeField] private Texture2D image;
    [SerializeField] private Optional<string> tutorialSceneName;
    [SerializeField] private float[] medalTimes = new float[4];

    public NextLevelStatus NLevelStatus => nextLevelStatus;
    public string CustomNextLevelName => customNextLevelName;
    public Vector3 SpawnPosition => spawnPosition;
    public float RotationAngle => rotationAngle;
    public Quaternion VelocityDirection => velocityDirection;
    public float Speed => speed;
    public string Title => title;
    public Texture2D Image => image;
    public Optional<string> TutorialSceneName => tutorialSceneName;
    public bool SnapDownAtStart => snapDownAtStart;
    public LayerMask PlayerLayerMask => layerMask;
    public IList<float> MedalTimes => Array.AsReadOnly(medalTimes);

    public void SetSpawnTransform(Transform transform)
    {
        spawnPosition = transform.position;
        rotationAngle = Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up);
        velocityDirection = Quaternion.LookRotation(transform.forward, transform.up);
    }
}