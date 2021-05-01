using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Scriptable Objects/Sound")]
public class Sound : ScriptableObject
{
    [Tooltip("This is for stuff like Metal 1 and Metal 2, leave the name as Metal in that case. Otherwise blank")]
    public string groupingName = "";

    public string filename;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0f, 0.5f)]
    public float volumeVarience = 0f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [Range(0f, 1f)]
    public float pitchVarience = 0f;

    public bool isLoop;
    public bool hasCooldown;

    [Min(0)]
    public float delay = 0f;

    [Range(0, 256), Tooltip("Lower is higher priority")]
    public int priority = 128;

    [HideInInspector] public AudioSource source;
}