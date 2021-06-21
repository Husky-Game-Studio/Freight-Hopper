using UnityEngine.SceneManagement;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private string nextLevelName = "0 0";

    private void Awake()
    {
        nextLevelName = LevelController.Instance.GetNextLevel();
    }

    private void OnTriggerEnter(UnityEngine.Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            LevelLoader.LoadLevel(nextLevelName);
        }
    }
}