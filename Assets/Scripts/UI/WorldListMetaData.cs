using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/Level/World List Meta Data"), System.Serializable]
public class WorldListMetaData : ScriptableObject
{
    [SerializeField] private WorldMetaData[] worlds = new WorldMetaData[9];

    public IReadOnlyList<WorldMetaData> Worlds => worlds;

    public WorldMetaData GetWorld(string name){
        return Worlds.Single(s => s != null && s.name == name);
    }
}
