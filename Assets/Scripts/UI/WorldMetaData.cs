using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(menuName = "Scriptable Objects/Level/World Meta Data"), System.Serializable]
public class WorldMetaData : ScriptableObject
{
    [SerializeField] private LevelData[] levels = new LevelData[10];

    public IReadOnlyList<LevelData> Levels => levels;
}