using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    public LevelName CurrentLevelName => levelName;
    [SerializeField, ReadOnly] private LevelName levelName;
    [SerializeField] private LevelData levelData;
    public Vector3 SpawnPosition => levelData.spawnPosition;

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
        Player.PlayerLoadedIn += UnlockAbilities;
    }

    private void OnDestroy()
    {
        Player.PlayerLoadedIn -= Respawn;
        Player.PlayerLoadedIn -= UnlockAbilities;
    }

    public void UnlockAbilities()
    {
        Player.Instance.GetComponent<PlayerAbilities>().SetActiveAbilities(levelData.activeAbilities);
    }

    public void Respawn()
    {
        GameObject player = Player.Instance.gameObject;
        if (player == null)
        {
            Debug.LogWarning("Can't find player");
        }
        player.transform.position = SpawnPosition;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void Respawn(InputAction.CallbackContext context)
    {
        Respawn();
    }
}