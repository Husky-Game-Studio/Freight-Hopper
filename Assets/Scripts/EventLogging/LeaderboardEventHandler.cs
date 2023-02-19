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
            EventBoat.NewBestTime += UploadTimes;
            switch (testCase)
            {
                case 1:
                    Debug.Log("Uploading preset seconds.");
                    UploadTimes("who cares", testSeconds);
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

    public static void UploadTimes(string level, float time)
    {
        leaderboardHandlers[level] = new SteamTrain.SteamLeaderboardHandler
        {
            newScore = (int)(time * 1000)
        };
        leaderboardHandlers[level].FindLeaderboardAndUploadScore(level);
    }

    public IEnumerator GetTimes(string level, int amount,
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
                Debug.Log("Leaderboard?");
                result.AddRange(leaderboardHandlers[level].readableLeaderboard);
                for (int j = 0; j < result.Count; ++j)
                {
                    Debug.Log(result[j].playerName);
                }
                Debug.Log("Leaderboard End");
            }
        }

        leaderboardHandlers[level].FindLeaderboardAndFriendScores(level);

        yield return new WaitWhile(delegate { return leaderboardHandlers[level].findingLeaderboard; });
        if (leaderboardHandlers[level].foundLeaderboard)
        {
            yield return new WaitWhile(delegate { return leaderboardHandlers[level].downloadingLeaderboards; });
            if (leaderboardHandlers[level].readLeaderboards)
            {
                Debug.Log("Friend Leaderboard?");
                resultFriends.AddRange(leaderboardHandlers[level].readableLeaderboard);
                for (int j = 0; j < resultFriends.Count; ++j)
                {
                    Debug.Log(resultFriends[j].playerName);
                }
                Debug.Log("Friend Leaderboard End");
            }
        }
        yield return null;
    }

    public IEnumerator GetRelativeTimes(string level, int amount,
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
                Debug.Log("Leaderboard?");
                result.AddRange(leaderboardHandlers[level].readableLeaderboard);
                for (int j = 0; j < result.Count; ++j)
                {
                    Debug.Log(result[j].playerName);
                }
                Debug.Log("Leaderboard End");
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
                Debug.Log("Friend Leaderboard?");
                resultFriends.AddRange(leaderboardHandlers[level].readableLeaderboard);
                for (int j = 0; j < resultFriends.Count; ++j)
                {
                    Debug.Log(resultFriends[j].playerName);
                }
                Debug.Log("Friend Leaderboard End");
            }
        }
        yield return null;
    }
}
