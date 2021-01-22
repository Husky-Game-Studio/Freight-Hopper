using UnityEngine.SceneManagement;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(UnityEngine.Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            SceneManager.LoadScene(GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelSettings>().NextLevelName);
            SceneManager.LoadScene("DefaultScene", LoadSceneMode.Additive);
        }
    }
}