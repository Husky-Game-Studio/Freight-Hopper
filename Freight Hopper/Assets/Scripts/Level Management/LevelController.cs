using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    public Vector3 SpawnPosition => levelData.spawnPosition;

    public LevelName CurrentLevelName => levelName;
    [SerializeField, ReadOnly] private LevelName levelName;

    public static LevelController Instance => instance;
    private static LevelController instance;

    private void OnValidate()
    {
        if (levelData != null)
        {
            levelName = new LevelName(SceneManager.GetActiveScene().name);
        }
    }

    public string GetNextLevel()
    {
        if (levelData.nextLevelStatus == LevelData.NextLevelStatus.NextLevel)
        {
            return levelName.NextLevel();
        }
        if (levelData.nextLevelStatus == LevelData.NextLevelStatus.NextWorld)
        {
            return levelName.NextWorld();
        }
        return levelName.CurrentLevel();
    }

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
        Player.PlayerLoadedIn += Respawn;
    }

    private void OnDestroy()
    {
        Player.PlayerLoadedIn -= Respawn;
    }

    public void Respawn()
    {
        // This shouldn't be done, but I am not sure what else to do for now
        GameObject player = Player.Instance.gameObject;
        if (player == null)
        {
            Debug.Log("Can't find player");
        }
        player.transform.position = SpawnPosition;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void Respawn(InputAction.CallbackContext context)
    {
        Respawn();
    }
}