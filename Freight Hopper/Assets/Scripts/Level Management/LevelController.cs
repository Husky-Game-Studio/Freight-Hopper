using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    public LevelName CurrentLevelName => levelName;
    [SerializeField, ReadOnly] private LevelName levelName;
    [SerializeField] private LevelData levelData;

    public static event Action PlayerRespawned;

    public static event Action LevelLoadedIn;

    public static LevelController Instance => instance;
    private static LevelController instance;

    private void OnDrawGizmosSelected()
    {
        GizmosExtensions.DrawGizmosArrow(levelData.spawnPosition, Vector3.forward);
    }

    private void OnValidate()
    {
        if (levelData != null)
        {
            Scene levelScene = SceneManager.GetActiveScene();

#if UNITY_EDITOR
            string playerSceneName = "DefaultScene";
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (!playerSceneName.Equals(SceneManager.GetSceneAt(i).name))
                {
                    levelScene = SceneManager.GetSceneAt(i);
                }
            }
#endif
            levelName = new LevelName(levelScene.name);
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
        LevelLoadedIn?.Invoke();
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
        player.transform.position = levelData.spawnPosition;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        PlayerRespawned?.Invoke();
        //player.transform.Rotate(Vector3.up, levelData.rotationAngle); Doesn't work due to camera
    }

    public void Respawn(InputAction.CallbackContext context)
    {
        Respawn();
    }
}