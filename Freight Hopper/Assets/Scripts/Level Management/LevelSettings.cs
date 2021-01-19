using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSettings : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;
    public Vector3 SpawnPosition => spawnPosition;

    [SerializeField] private string currentLevelName;
    public string CurrentLevelName => currentLevelName;

    [SerializeField] private string nextLevelName;
    public string NextLevelName => nextLevelName;

    private void Awake()
    {
        currentLevelName = SceneManager.GetActiveScene().name;
    }
}