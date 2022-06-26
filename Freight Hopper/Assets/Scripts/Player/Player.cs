using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public struct Modules
    {
        public Gravity gravity;
        public Friction friction;
        public Rigidbody rigidbody;
        public CollisionManagement collisionManagement;
        public RigidbodyLinker rigidbodyLinker;
        public SoundManager soundManager;
        public EdgeCorrectionCollision edgeCorrectionCollision;
    }
    public Modules modules;

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

    private void OnDisable()
    {
        loadedIn = false;
    }
}