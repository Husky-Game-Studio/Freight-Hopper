using UnityEngine;

[CreateAssetMenu(fileName = "Objects", menuName = "Scriptable Objects/Object Collection"), System.Serializable]
public class ObjectCollection : ScriptableObject
{
    public LevelEditorObjects[] objects;
}
