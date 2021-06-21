using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static event Action PlayerLoadedIn;

    private static Player instance;

    public static bool loadedIn = false;

    // If there is a null reference error try subscribing to the PlayerLoadedIn event
    public static Player Instance { get => instance; set => _ = instance; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        PlayerLoadedIn?.Invoke();
        loadedIn = true;
    }
}