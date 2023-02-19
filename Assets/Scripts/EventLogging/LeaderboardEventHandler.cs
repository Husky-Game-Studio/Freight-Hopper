using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardEventHandler : MonoBehaviour
{   
    private void Start()
    {
        EventBoat.NewBestTime += BeginTimeUploadingProcess;
    }

    public void FindLeaderboard(string level)
    {
        SteamTrain.SteamLeaderboardHandler.FindLeaderboard(level);
    }

    public void BeginTimeUploadingProcess(string level, float time)
    {
        SteamTrain.SteamLeaderboardHandler.UploadTimeBestTime(time);
    }
}
