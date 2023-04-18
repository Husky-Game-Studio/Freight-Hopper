using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

public class LevelController : MonoBehaviour
{
    private bool respawning;
    public LevelName CurrentLevelName => levelName;
    [SerializeField] private bool spawnPlayerHigh;
    [SerializeField, ReadOnly] private LevelName levelName;
    public Transform playerSpawnTransform;
    #if UNITY_EDITOR
    public int editorCheckpointIndex = 0;
    public List<Transform> editorCheckpoints;
    #endif
    [SerializeField] private float spawnSnapSmoothing = 0.001f;
    [SerializeField] private LevelData levelData;

    public LevelData LevelData => levelData;

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

    public string GetVersionedLevel => levelName.VersionedCurrentLevel(levelData);

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
        foreach (WorldMetaData world in levelData.World.WorldsList.Worlds) 
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

        Vector3 position = playerSpawnTransform.position;
        Quaternion rotation = Quaternion.LookRotation(playerSpawnTransform.forward);
        if (spawnPlayerHigh)
        {
            position = playerSpawnTransform.position + (this.transform.up * highHeight);
        }
#if UNITY_EDITOR
        if (editorCheckpointIndex < editorCheckpoints.Count && editorCheckpointIndex < 0)
        {
            position = editorCheckpoints[editorCheckpointIndex].position;
        }
#endif
        
        InstantiationParameters parameters = new InstantiationParameters(position, rotation, null);
        yield return Addressables.InstantiateAsync("PlayerPrefab.prefab", parameters);
        Player player = Player.Instance;
        if (player == null)
        {
            Debug.LogWarning("Can't find player");
        }

        Ray ray = new Ray(player.transform.position - Vector3.up, -Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, LayerMask.GetMask("Ignore Raycast", "NoHover")))
        {
            player.transform.position = hit.point + (Vector3.up * 2) + (Vector3.up * spawnSnapSmoothing);
            if(hit.rigidbody != null){
                player.modules.rigidbodyLinker.UpdateLink(hit.rigidbody);
            }
        }
        
        
        LevelLoadedIn?.Invoke();
        yield return Addressables.InstantiateAsync("Assets/Prefabs/SteamManager.prefab");
        
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