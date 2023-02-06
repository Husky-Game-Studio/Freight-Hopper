using UnityEngine;
using System;

public class Goal : MonoBehaviour
{
    public static Action LevelComplete;

    static bool completedLevel;
    public static bool CompletedLevel => completedLevel;

    private void Awake()
    {
        completedLevel = false;
    }

    private void OnTriggerEnter(UnityEngine.Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            LevelComplete?.Invoke();
            completedLevel = true;
        }
    }
}