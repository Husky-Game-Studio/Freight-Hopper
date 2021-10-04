using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Level/World Meta Data"), System.Serializable]
public class WorldMetaData : ScriptableObject
{
    [SerializeField] private LevelData[] levels = new LevelData[10];

    public LevelData[] Levels => levels.Clone() as LevelData[];
}