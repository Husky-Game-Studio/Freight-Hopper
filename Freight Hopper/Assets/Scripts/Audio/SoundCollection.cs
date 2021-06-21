using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Scriptable Objects/Sound Collection"), System.Serializable]
public class SoundCollection : ScriptableObject
{
    public Sound[] sounds;
}