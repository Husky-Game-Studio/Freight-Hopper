using UnityEngine;
using System.Collections.Generic;
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
    [SerializeField] private float speed;
    [SerializeField] private bool snapDownAtStart = true;
    [SerializeField] private bool enabled = false;
    [SerializeField] private LayerMask layerMask;

    [Header("Meta Data")]
    [SerializeField] private string title;
    [SerializeField] private Texture2D image;
    [SerializeField] private Optional<string> tutorialSceneName;
    [SerializeField] private float[] medalTimes = new float[4];

    public NextLevelStatus NLevelStatus => nextLevelStatus;
    public string CustomNextLevelName => customNextLevelName;
    public float Speed => speed;
    public string Title => title;
    public Texture2D Image => image;
    public bool SnapDownAtStart => snapDownAtStart;
    public bool Enabled => enabled;
    public LayerMask PlayerLayerMask => layerMask;
    public IList<float> MedalTimes => Array.AsReadOnly(medalTimes);
}