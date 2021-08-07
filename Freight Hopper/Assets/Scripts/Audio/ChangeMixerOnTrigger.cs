using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ChangeMixerOnTrigger : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup mixerToFadeInto;
    private bool switched = false;
    private AudioMixerGroup originalMixer;

    private void Awake()
    {
        originalMixer = MusicManager.Instance.MixerGroup;
        switched = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        if (!switched)
        {
            switched = true;
            MusicManager.Instance.ChangeMixer(mixerToFadeInto);
        }
        else
        {
            switched = false;
            MusicManager.Instance.ChangeMixer(originalMixer);
        }
    }
}