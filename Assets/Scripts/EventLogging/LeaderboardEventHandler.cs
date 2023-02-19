using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LeaderboardEventHandler : MonoBehaviour
{
    public List<string> leaderboards = new List<string>();
    public int testCase = 0;
    public int testSeconds = 30;
    private static Dictionary<string, SteamTrain.SteamLeaderboardHandler> leaderboardHandlers = new Dictionary<string, SteamTrain.SteamLeaderboardHandler>();

    private void Start()
    {
        if (SteamTrain.SteamAchievementHandler.statsRetrieved)
        {
            EventBoat.NewBestTime += UploadTimes;
            switch (testCase)
            {
                case 1:
                    Debug.Log("Uploading preset seconds.");
                    UploadTimes("who cares", testSeconds);
                    break;
            }
        }
        else
            Debug.Log("Did not subscribe events because Steam Stats were not retrieved.");
    }

    public void UploadTimes(string level, float time)
    {
        foreach (string s in leaderboards)
        {
            leaderboardHandlers[s] = new SteamTrain.SteamLeaderboardHandler
            {
                newScore = (int)(time * 1000)
            };
            leaderboardHandlers[s].FindLeaderboardAndUploadScore(s);
        }
    }
}
