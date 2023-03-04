using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LeaderboardEventHandler : MonoBehaviour
{
    public int testCase = 0;
    public int testSeconds = 30;
    private static Dictionary<string, SteamTrain.SteamLeaderboardHandler> leaderboardHandlers = new Dictionary<string, SteamTrain.SteamLeaderboardHandler>();

    private void Start()
    {
        if (SteamTrain.SteamAchievementHandler.statsRetrieved)
        {
            EventBoat.OnLevelComplete += UploadTimes;
            switch (testCase)
            {
                case 1:
                    //Debug.Log("Uploading preset seconds.");
                    //UploadTimes("who cares", testSeconds);
                    break;
                case 2:
                    Debug.Log("Printing leaderboards.");
                    StartCoroutine(GetTimes("1 1", 2, new List<SteamTrain.LeaderboardEntry>(), new List<SteamTrain.LeaderboardEntry>()));
                    break;
                case 3:
                    Debug.Log("Printing relative leaderboards");
                    StartCoroutine(GetRelativeTimes("1 1", 5, new List<SteamTrain.LeaderboardEntry>(), new List<SteamTrain.LeaderboardEntry>()));
                    break;
            }
        }
        else
            Debug.Log("Did not subscribe events because Steam Stats were not retrieved.");
    }

    public static void UploadTimes(LevelCompleteData leveldata)
    {
        leaderboardHandlers[leveldata.Level] = new SteamTrain.SteamLeaderboardHandler
        {
            newScore = Mathf.RoundToInt(leveldata.Time * 1000f)
        };
        leaderboardHandlers[leveldata.Level].FindLeaderboardAndUploadScore(leveldata.Level);
    }

    public static IEnumerator GetTimes(string level, int amount,
                                List<SteamTrain.LeaderboardEntry> result,
                                List<SteamTrain.LeaderboardEntry> resultFriends)
    {
        leaderboardHandlers[level] = new SteamTrain.SteamLeaderboardHandler
        {
                firstLimit = 1,
                secondLimit = amount
        };
        leaderboardHandlers[level].FindLeaderboardAndDownloadScores(level);

        yield return new WaitWhile(delegate { return leaderboardHandlers[level].findingLeaderboard; });
        if (leaderboardHandlers[level].foundLeaderboard)
        {
            yield return new WaitWhile(delegate { return leaderboardHandlers[level].downloadingLeaderboards; });
            if (leaderboardHandlers[level].readLeaderboards)
            {
                result.AddRange(leaderboardHandlers[level].readableLeaderboard);
            }
        }

        leaderboardHandlers[level].FindLeaderboardAndFriendScores(level);

        yield return new WaitWhile(delegate { return leaderboardHandlers[level].findingLeaderboard; });
        if (leaderboardHandlers[level].foundLeaderboard)
        {
            yield return new WaitWhile(delegate { return leaderboardHandlers[level].downloadingLeaderboards; });
            if (leaderboardHandlers[level].readLeaderboards)
            {
                resultFriends.AddRange(leaderboardHandlers[level].readableLeaderboard);
            }
        }
        yield return null;
    }

    public static IEnumerator GetRelativeTimes(string level, int amount,
                                        List<SteamTrain.LeaderboardEntry> result,
                                        List<SteamTrain.LeaderboardEntry> resultFriends)
    {
        leaderboardHandlers[level] = new SteamTrain.SteamLeaderboardHandler
        {
            firstLimit = amount/2,
            secondLimit = amount/2
        };
        leaderboardHandlers[level].FindLeaderboardAndDownloadRelativeScores(level);

        yield return new WaitWhile(delegate { return leaderboardHandlers[level].findingLeaderboard; });
        if (leaderboardHandlers[level].foundLeaderboard)
        {
            yield return new WaitWhile(delegate { return leaderboardHandlers[level].downloadingLeaderboards; });
            if (leaderboardHandlers[level].readLeaderboards)
            {
                result.AddRange(leaderboardHandlers[level].readableLeaderboard);
            }
        }

        leaderboardHandlers[level] = new SteamTrain.SteamLeaderboardHandler
        {
            firstLimit = 1,
            secondLimit = amount
        };
        leaderboardHandlers[level].FindLeaderboardAndFriendScores(level);

        yield return new WaitWhile(delegate { return leaderboardHandlers[level].findingLeaderboard; });
        if (leaderboardHandlers[level].foundLeaderboard)
        {
            yield return new WaitWhile(delegate { return leaderboardHandlers[level].downloadingLeaderboards; });
            if (leaderboardHandlers[level].readLeaderboards)
            {
                resultFriends.AddRange(leaderboardHandlers[level].readableLeaderboard);
            }
        }
        yield return null;
    }
}
