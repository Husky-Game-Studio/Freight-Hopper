using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Ore;

public class LeaderboardEventHandler : OSingleton<LeaderboardEventHandler>
{
    public int testCase = 0;
    public string testLevel = "1 9";
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
                    Debug.Log("Uploading bytearray 0000 to the leaderboard.");
                    UploadTimeAndFile(new LevelCompleteData() { Level = testLevel, Time = testSeconds}, new byte[4]);
                    break;
                case 2:
                    Debug.Log("Printing leaderboards.");
                    StartCoroutine(GetTimes(testLevel, 2, new List<SteamTrain.LeaderboardEntry>(), new List<SteamTrain.LeaderboardEntry>()));
                    break;
                case 3:
                    Debug.Log("Printing relative leaderboards.");
                    StartCoroutine(GetRelativeTimes(testLevel, 5, new List<SteamTrain.LeaderboardEntry>(), new List<SteamTrain.LeaderboardEntry>()));
                    break;
                case 4:
                    Debug.Log("Printing my score.");
                    StartCoroutine(GetMyUserTime(testLevel, new SteamTrain.LeaderboardEntry()));
                    break;
                case 5:
                    Debug.Log("Forcibly updating score");
                    UploadTimeForced(new LevelCompleteData() { Level = testLevel, Time = testSeconds });
                    break;

            }
        }
        else
            Debug.Log("Did not subscribe events because Steam Stats were not retrieved.");
    }

    public static void UploadTimeForced(LevelCompleteData leveldata)
    {
        leaderboardHandlers[leveldata.Level] = new SteamTrain.SteamLeaderboardHandler
        {
            newScore = Mathf.RoundToInt(leveldata.Time * 1000f)
        };
        leaderboardHandlers[leveldata.Level].FindLeaderboardAndForceUploadScore(leveldata.Level);
    }

    public static void UploadTimeAndFile(LevelCompleteData leveldata, byte[] data)
    {
        leaderboardHandlers[leveldata.Level] = new SteamTrain.SteamLeaderboardHandler
        {
            newScore = Mathf.RoundToInt(leveldata.Time * 1000f),
            leaderboardData = data,
            leaderboardFName = "Level" + leveldata.Level.Replace(" ","_") + "_BestReplay.txt"
        };
        leaderboardHandlers[leveldata.Level].FindLeaderboardAndUploadData(leveldata.Level);
    }

    public static IEnumerator GetMyUserTime(string level, SteamTrain.LeaderboardEntry result)
    {
        yield return GetTargetUserTimes(level, result, SteamManager.GetMySteamID());
    }

    public static IEnumerator GetTargetUserTimes(string level, SteamTrain.LeaderboardEntry result, ulong whomst)
    {
        leaderboardHandlers[level] = new SteamTrain.SteamLeaderboardHandler()
        {
            whomst = new CSteamID(whomst)
        };
        leaderboardHandlers[level].FindLeaderboardAndDownloadSomeGuysScore(level);

        yield return new WaitWhile(delegate { return leaderboardHandlers[level].findingLeaderboard; });
        if (leaderboardHandlers[level].foundLeaderboard)
        {
            yield return new WaitWhile(delegate { return leaderboardHandlers[level].downloadingLeaderboards; });
            if (leaderboardHandlers[level].readLeaderboards)
            {
                result.Copy(leaderboardHandlers[level].readableLeaderboard[0]);
            }
        }
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
        leaderboardHandlers[level].readableLeaderboard.Clear();
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
        leaderboardHandlers[level].readableLeaderboard.Clear();
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
