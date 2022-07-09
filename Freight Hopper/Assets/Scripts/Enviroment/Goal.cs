using UnityEngine;
using System;

public class Goal : MonoBehaviour
{
    private static Goal instance;
    public static Goal Instance => instance;

    private Action levelComplete;

    public void SetLevelCompleteScreen(Action levelComplete)
    {
        this.levelComplete = levelComplete;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Only one goal is allowed, please write a goal manager script for more");
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(UnityEngine.Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            levelComplete.Invoke();
        }
    }
}