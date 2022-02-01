using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private bool respawning = false;
    public LevelName CurrentLevelName => levelName;
    [SerializeField] private bool spawnPlayerHigh;
    [SerializeField, ReadOnly] private LevelName levelName;
    [SerializeField] private Transform playerSpawnTransform;
    public LevelData levelData;

    private const int highHeight = 999999;
    private static LevelData lastLeveData;

    public static event Action PlayerRespawned;

    public static event Action LevelLoadedIn;

    public static LevelController Instance => instance;
    private static LevelController instance;

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(levelData.SpawnPosition, 2);
    }

#endif

    private void OnValidate()
    {
        if (playerSpawnTransform != null)
        {
            levelData.SetSpawnTransform(playerSpawnTransform);
        }
    }

    public void LoadNextLevel()
    {
        string nextLevelName = GetNextLevel();
        if (nextLevelName != "MainMenu")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SceneLoader.LoadLevel(nextLevelName);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(nextLevelName);
        }
    }

    [ContextMenu("UpdateSpawnTransform")]
    public void UpdateSpawn()
    {
        if (playerSpawnTransform == null)
        {
            Debug.LogWarning("Please set a transform in the inspector first for playerSpawnTransform");
            return;
        }
        levelData.SetSpawnTransform(playerSpawnTransform);
    }

    public string GetNextLevel()
    {
        if (levelData.NLevelStatus == LevelData.NextLevelStatus.NextLevel)
        {
            return levelName.NextLevel();
        }
        if (levelData.NLevelStatus == LevelData.NextLevelStatus.NextWorld)
        {
            return levelName.NextWorld();
        }
        if (levelData.NLevelStatus == LevelData.NextLevelStatus.Menu)
        {
            return "MainMenu";
        }
        if (levelData.NLevelStatus == LevelData.NextLevelStatus.Custom)
        {
            return levelData.CustomNextLevelName;
        }
        return levelName.CurrentLevel();
    }

    private void Awake()
    {
        respawning = false;
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
        bool defaultSceneAlreadyLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals("DefaultScene"))
            {
                defaultSceneAlreadyLoaded = true;
            }
        }
        if (!defaultSceneAlreadyLoaded)
        {
            SceneLoader.LoadPlayerScene();
        }

        LevelLoadedIn?.Invoke();
    }

    private void OnDestroy()
    {
        Player.PlayerLoadedIn -= UnlockAbilities;
        lastLeveData = levelData;
    }

    public void UnlockAbilities()
    {
        if (lastLeveData != null)
        {
            levelData.UpdateToLastLevelsAbilities(lastLeveData.DefaultAbilites, lastLeveData.ActiveAbilities);
        }

        Player.Instance.GetComponent<PlayerAbilities>().SetActiveAbilities(levelData.ActiveAbilities);
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
            player.transform.position = levelData.SpawnPosition + (this.transform.up * highHeight);
        }
        else
        {
            player.transform.position = levelData.SpawnPosition;
        }
        Vector3 gravityDirection = CustomGravity.GetUpAxis(player.transform.position);
        player.transform.rotation = Quaternion.LookRotation(Vector3.forward, gravityDirection) *
            Quaternion.AngleAxis(levelData.RotationAngle, Vector3.up);

        if (levelData.SnapDownAtStart)
        {
            Ray ray = new Ray(player.transform.position - player.transform.up * 2, -player.transform.up);
            if (Physics.Raycast(ray, out RaycastHit hit, levelData.PlayerLayerMask))
            {
                float dist = hit.distance;
                player.transform.position += ray.direction * dist;
            }
        }

        player.GetComponent<Rigidbody>().velocity = levelData.VelocityDirection * Vector3.forward * levelData.Speed;
    }

    // Respawns player. Reloads scene for now. Respawning var is used to prevent spamming of respawn button
    public void Respawn()
    {
        if (respawning)
        {
            return;
        }

        respawning = true;
        SceneLoader.LoadLevel(levelName.CurrentLevel());
        PlayerRespawned?.Invoke();

        //player.transform.Rotate(Vector3.up, levelData.rotationAngle); Doesn't work due to camera
    }

    public void Respawn(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (PauseMenu.Instance.Paused)
        {
            return;
        }
        Respawn();
    }
}