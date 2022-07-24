using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelSelectScreenOpener : MonoBehaviour
{
    [SerializeField]GameObject[] disable;
    [SerializeField]GameObject levelScreenUI;
    private static string lastScene = "";
    public static void SwitchSceneFlag(string scene)
    {
        lastScene = scene;
    }
    public void Start()
    {
        if (string.IsNullOrEmpty(lastScene))
        {
            return;
        }

        foreach (GameObject gameObject in disable)
        {
            gameObject.SetActive(false);
        }
        levelScreenUI.SetActive(true);
    }
}

