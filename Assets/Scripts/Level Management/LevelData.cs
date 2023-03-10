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
    [SerializeField] private bool enabled = false;

    [Header("Meta Data")]
    [SerializeField] private string title;
    [SerializeField] private int version = 1;
    [SerializeField] private Texture2D image;
    [SerializeField] private float[] medalTimes = new float[4];
    [SerializeField] private WorldMetaData world;

    public WorldMetaData World          =>  world;
    public NextLevelStatus NLevelStatus =>  nextLevelStatus;
    public string CustomNextLevelName   =>  customNextLevelName;
    public int Version                  =>  version;
    public string Title                 =>  title;
    public Texture2D Image              =>  image;
    public bool Enabled                 =>  enabled;
    public IList<float> MedalTimes      =>  Array.AsReadOnly(medalTimes);
}