using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Level/World Meta Data"), System.Serializable]
public class WorldMetaData : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField] private int worldID;
    public string Title => title;
    public int WorldID => worldID;
}