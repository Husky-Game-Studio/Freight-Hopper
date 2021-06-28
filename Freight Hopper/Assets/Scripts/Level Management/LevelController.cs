using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public LevelName CurrentLevelName => levelName;
    [SerializeField] private bool spawnPlayerHigh;
    [SerializeField, ReadOnly] private LevelName levelName;
    [SerializeField] private LevelData levelData;

    private const int highHeight = 999999;

    public static event Action PlayerRespawned;

    public static event Action LevelLoadedIn;

    public static LevelController Instance => instance;
    private static LevelController instance;

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(levelData.spawnPosition, 1);
    }

#endif

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
        if (levelData.nextLevelStatus == LevelData.NextLevelStatus.Menu)
        {
            return "MainMenu";
        }
        return levelName.CurrentLevel();
    }

    private void Awake()
    {
        if (levelData != null)
        {
            Scene levelScene = SceneManager.GetActiveScene();
            levelName = new LevelName(levelScene.name);
        }
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        Player.PlayerLoadedIn += ResetPlayerPosition;
        Player.PlayerLoadedIn += UnlockAbilities;
        SceneLoader.LoadPlayerScene();
        LevelLoadedIn?.Invoke();
    }

    private void OnDestroy()
    {
        Player.PlayerLoadedIn -= UnlockAbilities;
    }

    public void UnlockAbilities()
    {
        Player.Instance.GetComponent<PlayerAbilities>().SetActiveAbilities(levelData.activeAbilities);
    }

    private void ResetPlayerPosition()
    {
        GameObject player = Player.Instance.gameObject;
        if (player == null)
        {
            Debug.LogWarning("Can't find player");
        }
        if (spawnPlayerHigh)
        {
            player.transform.position = levelData.spawnPosition + transform.up * highHeight;
        }
        else
        {
            player.transform.position = levelData.spawnPosition;
        }

        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void Respawn()
    {
        SceneLoader.LoadLevel(levelName.CurrentLevel());
        PlayerRespawned?.Invoke();
        //player.transform.Rotate(Vector3.up, levelData.rotationAngle); Doesn't work due to camera
    }

    public void Respawn(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Respawn();
    }
}