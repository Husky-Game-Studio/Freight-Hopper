using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioDropdown : MonoBehaviour
{
    AudioSource backgroundMusic;
    AudioClip music; 
    public Dropdown audioDropdown;

    //public GameObject targetAccess;
    public void Start()
    {
        backgroundMusic = GetComponent<AudioSource>();

    }
    // Update is called once per frame
    public void Update()
    {
        // Railways -- Main Menu
        if (audioDropdown.value == 1)
        {
            music = (AudioClip)Resources.Load("LevelEditorMusic/Railways");
            backgroundMusic.clip = music;
            backgroundMusic.Play();
            audioDropdown.value = 0; 
        }

        // Heatwave 
        if (audioDropdown.value == 2)
        {
            music = (AudioClip)Resources.Load("LevelEditorMusic/Heatwave");
            backgroundMusic.clip = music;
            backgroundMusic.Play();
            audioDropdown.value = 0;
        }
        // Skid
        if (audioDropdown.value == 3)
        {
            music = (AudioClip)Resources.Load("LevelEditorMusic/Skid");
            backgroundMusic.clip = music;
            backgroundMusic.Play();
            audioDropdown.value = 0;
        }
        // Scramble 
        if (audioDropdown.value == 4)
        {
            music = (AudioClip)Resources.Load("LevelEditorMusic/Scramble");
            backgroundMusic.clip = music;
            backgroundMusic.Play();
            audioDropdown.value = 0;
        }


    }
}
