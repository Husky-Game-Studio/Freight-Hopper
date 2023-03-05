using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LevelController : MonoBehaviour
{
    private bool respawning;
    public LevelName CurrentLevelName => levelName;
    [SerializeField] private bool spawnPlayerHigh;
    [SerializeField, ReadOnly] private LevelName levelName;
    [SerializeField] public Transform playerSpawnTransform;
    [SerializeField] private float spawnSnapSmoothing = 0.001f;
    public LevelData levelData;
    public WorldListMetaData worldListMetaData;

    private const int highHeight = 999999;

    public static event Action PlayerRespawned;

    public static event Action LevelLoadedIn;

    public static LevelController Instance => instance;
    private static LevelController instance;

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerSpawnTransform.position, 2);
    }

#endif

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
    public void LoadRandomLevel()
    {
        List<LevelData> validLevelData = new List<LevelData>();
        foreach (WorldMetaData world in worldListMetaData.Worlds) 
        {
            if (world == null) continue;
            foreach (LevelData level in world.Levels)
            {
                if (level != null && level.Enabled)
                {
                    validLevelData.Add(level);
                }
            }
        }
        int randomLevel = UnityEngine.Random.Range(0, validLevelData.Count);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneLoader.LoadLevel(validLevelData[randomLevel].name);
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

    private IEnumerator Start()
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

        yield return Addressables.InstantiateAsync("PlayerPrefab.prefab");
        ResetPlayerPosition();
        Player.PlayerLoadedIn += ResetPlayerPosition;
        LevelLoadedIn?.Invoke();
    }



    private void OnDestroy()
    {
        Player.PlayerLoadedIn -= ResetPlayerPosition;
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
            player.transform.position = playerSpawnTransform.position + (this.transform.up * highHeight);
        }
        else
        {
            player.transform.position = playerSpawnTransform.position;
        }
        player.transform.forward = playerSpawnTransform.forward;

        if (levelData.SnapDownAtStart)
        {
            Ray ray = new Ray(player.transform.position - Vector3.up, -Vector3.up);
            if (Physics.Raycast(ray, out RaycastHit hit, levelData.PlayerLayerMask))
            {
                player.transform.position = hit.point + (Vector3.up*2) + (Vector3.up * spawnSnapSmoothing);
            }
        }

        player.GetComponent<Rigidbody>().velocity = player.transform.forward * levelData.Speed;
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
    }

    public void Respawn(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (InGameStates.Instance.StateIs(InGameStates.States.Paused))
        {
            return;
        }
        Respawn();
    }
}