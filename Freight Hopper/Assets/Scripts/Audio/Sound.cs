using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Scriptable Objects/Sound")]
public class Sound : ScriptableObject
{
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

    [HideInInspector] public AudioSource source;
}