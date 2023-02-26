using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [Serializable]
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

    // If there is a null reference error try subscribing to the PlayerLoadedIn event
    public static Player Instance => instance;

    private void OnEnable()
    {
        instance = this;
        PlayerLoadedIn?.Invoke();
    }
    private void OnDisable()
    {
        instance = null;
    }
}