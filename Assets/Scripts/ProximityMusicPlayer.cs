using System;
using UnityEngine;

public class ProximityMusicPlayer : MonoBehaviour
{
    [SerializeField] float noMusicSphere;
    [SerializeField, Min(0)] float transitionDistance;
    [SerializeField] SoundManager soundManager;
    
    float transitionSphereMin;
    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, noMusicSphere);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, transitionSphereMin);
    }
    void Awake()
    {
        transitionSphereMin = noMusicSphere - transitionDistance;
    }
    void OnValidate()
    {
        if (noMusicSphere < transitionDistance) noMusicSphere = transitionDistance;
        transitionSphereMin = noMusicSphere - transitionDistance;
    }
    
    bool isPlaying = false;
    float dist;
    void Update()
    {
        if (Player.Instance == null) return;
        if (MusicManager.Instance == null) return;
        
        dist = Vector3.Distance(Player.Instance.transform.position, transform.position);
        if (dist > noMusicSphere)
        {
            MusicManager.Instance.ResetVolumePercent();
            soundManager.MuteVolume();
            if (isPlaying)
            {
                soundManager.Stop("RobertoTheme");
                isPlaying = false;
            }
            
            return;
        }
       
        if (dist < transitionSphereMin)
        {
            MusicManager.Instance.MuteVolume();
            return;
        }
        if (!isPlaying)
        {
            soundManager.Play("RobertoTheme");
            soundManager.SetMaxDistance("RobertoTheme", transitionSphereMin);
        }
        isPlaying = true;
        soundManager.ResetVolumePercent();

        float t = Mathf.InverseLerp(transitionSphereMin, noMusicSphere, dist);
        MusicManager.Instance.ModifyVolumePercent(t);
    }
}
