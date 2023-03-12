using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteamTrain;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UILeaderboard : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
{
    [SerializeField] GameObject loadingIcon;

    [SerializeField] GameObject leaderboardEntryPrefab;
    [SerializeField] LoopScrollRect loopScrollRect;
    [SerializeField] UnityEngine.UI.Toggle toggleFriend;
    [SerializeField] int timesToGrab = 50;
    [SerializeField] TMP_Dropdown supportedVersionsDropdown;
    List<LeaderboardEntry> fullLeaderboard = new List<LeaderboardEntry>();
    List<LeaderboardEntry> friendLeaderboard = new List<LeaderboardEntry>();

    private LevelData currentLevelData;
    public bool IsFriendLeaderboard => toggleFriend.isOn;
    public static UILeaderboard Instance;

    void OnEnable(){
        Instance = this;
        loopScrollRect.prefabSource = this;
        loopScrollRect.dataSource = this;
        loadingIcon.gameObject.SetActive(true);
        StartCoroutine(Timeout());
        if(LevelController.Instance != null){
            currentLevelData = LevelController.Instance.LevelData;
            SetupDropdownVersions(currentLevelData);
        }
    }

    IEnumerator Timeout(){
        yield return new WaitForSecondsRealtime(3);
        loadingIcon.gameObject.SetActive(false);
    }

    // Only works in level scenes
    public void LoadCurrentSceneLeaderboard(){
        StartCoroutine(LoadRelativeLeaderboard(LevelController.Instance.GetVersionedLevel));
    }
    void SetupDropdownVersions(LevelData levelData)
    {
        supportedVersionsDropdown.ClearOptions();
        supportedVersionsDropdown.AddOptions(levelData.SupportedVersionsDisplayStrings());
    }
    public void UpdateDisplayedLeaderboardViaIndex(int index){
        Vector2Int ver = currentLevelData.SupportedVersions[index];
        StartCoroutine(LoadRelativeLeaderboard(LevelController.Instance.CurrentLevelName.VersionedCurrentLevel(ver.x, ver.y)));
    }

    public IEnumerator LoadRelativeLeaderboard(string level){
        loadingIcon.gameObject.SetActive(true);
        fullLeaderboard.Clear();
        friendLeaderboard.Clear();
        loopScrollRect.totalCount = 0;
        loopScrollRect.RefillCells();
        yield return LeaderboardEventHandler.GetRelativeTimes(level, timesToGrab, fullLeaderboard, friendLeaderboard);
        if(fullLeaderboard.Count == 0){
            yield return LeaderboardEventHandler.GetTimes(level, timesToGrab, fullLeaderboard, friendLeaderboard);
        }
        StopCoroutine(Timeout());
        loadingIcon.gameObject.SetActive(false);

        RefreshLeaderboard();
    }

    public void RefreshLeaderboard(){
        int count = fullLeaderboard.Count;
        if (IsFriendLeaderboard)
        {
            count = friendLeaderboard.Count;
        }

        loopScrollRect.totalCount = count;    
        loopScrollRect.RefillCells();
    }

    public LeaderboardEntry GetEntry(int index){
        if(IsFriendLeaderboard)
        {
            if (index >= friendLeaderboard.Count) return null;
            return friendLeaderboard[index];
        }
        if (index >= fullLeaderboard.Count) return null;
        return fullLeaderboard[index];
    }
    

    // Implement your own Cache Pool here. The following is just for example.
    Stack<Transform> pool = new Stack<Transform>();
    public GameObject GetObject(int index)
    {
        if (pool.Count == 0)
        {
            return Instantiate(leaderboardEntryPrefab);
        }
        Transform candidate = pool.Pop();
        candidate.gameObject.SetActive(true);
        return candidate.gameObject;
    }

    public void ReturnObject(Transform trans)
    {
        // Use `DestroyImmediate` here if you don't need Pool
        trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
        trans.gameObject.SetActive(false);
        trans.SetParent(transform, false);
        pool.Push(trans);
    }

    public void ProvideData(Transform transform, int idx)
    {
        transform.SendMessage("ScrollCellIndex", idx);
    }
}
