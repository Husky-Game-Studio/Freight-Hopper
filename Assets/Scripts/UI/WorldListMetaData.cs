using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/Level/World List Meta Data"), System.Serializable]
public class WorldListMetaData : ScriptableObject
{
    [SerializeField] private WorldMetaData[] worlds = new WorldMetaData[9];

    public IReadOnlyList<WorldMetaData> Worlds => worlds;
}
