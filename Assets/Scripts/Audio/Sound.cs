using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "Sound", menuName = "Scriptable Objects/Sound")]
public class Sound : ScriptableObject, IDisposable
{
    public enum ReleaseStrategy
    {
        Normal = 0,
        AlsoOnStop = 1
    }
    [Tooltip("This is for stuff like Metal 1 and Metal 2, leave the name as Metal in that case. Otherwise blank")]
    public string groupingName = "";

    public string filename;
    public AssetReference assetReference;
    public ReleaseStrategy releaseStrategy = ReleaseStrategy.Normal;
    public float clipLength = 0;

    [Range(0f, 1f)]
    public float volume = 1f;

    public float fadeInTime;
    public float fadeOutTime;

    [Range(0f, 0.5f)]
    public float volumeVarience;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [Range(0f, 1f)]
    public float pitchVarience;

    public bool isLoop;
    public bool hasCooldown;

    [Range(0f, 1f)]
    public float spatialBlend;

    [Min(0)]
    public float delay;

    [Range(0, 256), Tooltip("Lower is higher priority")]
    public int priority = 128;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (assetReference != null && assetReference.editorAsset != null && clipLength == 0)
        {
            clipLength = ((AudioClip)assetReference.editorAsset).length;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
#endif

    [NonSerialized] public AudioSource componentAudioSource;
    [NonSerialized] public AsyncOperationHandle<AudioClip> handle;
    public void Dispose()
    {
        if (componentAudioSource != null)
        {
            componentAudioSource.clip = null;
            Destroy(componentAudioSource);
            componentAudioSource = null;
        }
        if (handle.IsValid())
        {
            Addressables.Release(handle);
            handle = default;
        }
    }
}