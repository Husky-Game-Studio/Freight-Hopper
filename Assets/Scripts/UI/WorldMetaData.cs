using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Scriptable Objects/Level/World Meta Data"), System.Serializable]
public class WorldMetaData : ScriptableObject
{
    [SerializeField] private LevelData[] levels = new LevelData[10];

    public IReadOnlyList<LevelData> Levels => levels;

    public LevelData GetLevel(string name)
    {
        return Levels.Single(s => s != null && s.name == name);
    }
}