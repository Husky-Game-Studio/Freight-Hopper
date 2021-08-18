using UnityEngine;

[CreateAssetMenu(fileName = "Objects", menuName = "Scriptable Objects/List Object Collection"), System.Serializable]
public class ListObjectCollection : ScriptableObject
{
    public ObjectCollection[] objects;
}

