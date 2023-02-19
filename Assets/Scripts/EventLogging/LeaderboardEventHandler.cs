using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LeaderboardEventHandler : MonoBehaviour
{
    public List<string> leaderboards = new List<string>();

    private Dictionary<string, SteamTrain.SteamLeaderboardHandler> leaderboardHandlers = new Dictionary<string, SteamTrain.SteamLeaderboardHandler>();

    private void Start()
    {
        foreach(string s in leaderboards)
        {
            leaderboardHandlers[s] = new SteamTrain.SteamLeaderboardHandler();
            leaderboardHandlers[s].FindLeaderboard(s);
        }
        EventBoat.NewBestTime += UploadTimes;
    }

    public void UploadTimes(string level, float time)
    {
        foreach(string s in leaderboards)
        {
            Debug.Log(s + leaderboardHandlers[s].UploadTimeBestTime(time, OnUploadScore).ToString());
        }
    }

    public void OnUploadScore(LeaderboardScoreUploaded_t r, bool failure)
    {
        if (r.m_bSuccess == 1 && !failure)
        {
            Debug.Log("Score uploaded.");
            if (r.m_bScoreChanged == 1)
                Debug.Log("Updated score.");
        }
        else
            Debug.Log("Failed to upload score.");
    }

    public void OnDownloadScore(LeaderboardScoresDownloaded_t r, bool failure)
    {
        if (r.m_cEntryCount > 0 && !failure)
        {
            Debug.Log("Scores downloaded.");
        }
        else
            Debug.Log("Failed to download scores.");
    }
}
