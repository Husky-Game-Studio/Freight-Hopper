using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

namespace SteamTrain
{
    // https://stackoverflow.com/questions/37842524/how-to-do-leaderboards-in-steamworks-net
    // https://partner.steamgames.com/doc/features/leaderboards/guide
    public class SteamLeaderboardHandler
    {
        public SteamLeaderboard_t currentLeaderboard { get; private set; }
        private CallResult<LeaderboardFindResult_t> callResultFindLeaderboard = new CallResult<LeaderboardFindResult_t>();
        private CallResult<LeaderboardScoreUploaded_t> callResultUploadScore = new CallResult<LeaderboardScoreUploaded_t>();
        private CallResult<LeaderboardScoresDownloaded_t> callResultDownloadScore = new CallResult<LeaderboardScoresDownloaded_t>();

        public bool foundLeaderboard { get; private set; } = false;
        public int newScore;

        public List<LeaderboardEntry> readableLeaderboard = new List<LeaderboardEntry>();

        public void FindLeaderboardAndUploadScore(string name)
        {
            foundLeaderboard = false;
            SteamAPICall_t cb = SteamUserStats.FindLeaderboard(name);
            callResultFindLeaderboard.Set(cb, OnFindLeaderboardUploadScore);
        }

        public bool UploadTimeBestTime(int time)
        {
            if(foundLeaderboard)
            {
                // makes sure that it only keeps best
                SteamAPICall_t cb = SteamUserStats.UploadLeaderboardScore(currentLeaderboard,
                                                                          ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
                                                                          time, null, 0);
                callResultUploadScore.Set(cb, OnUploadScore);
                return true;
            }
            return false;
        }

        public bool UploadTimeForced(int time)
        {
            if (foundLeaderboard)
            {
                // makes sure that it forcibly updates
                SteamAPICall_t cb = SteamUserStats.UploadLeaderboardScore(currentLeaderboard,
                                                                          ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate,
                                                                          time, null, 0);
                callResultUploadScore.Set(cb, OnUploadScore);
                return true;
            }
            return false;
        }

        public bool DownloadScores(CallResult<LeaderboardScoresDownloaded_t>.APIDispatchDelegate del)
        {
            if (foundLeaderboard)
            {
                SteamAPICall_t cb = SteamUserStats.DownloadLeaderboardEntries(currentLeaderboard,
                                                                          ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal,
                                                                          1, 10);
                callResultDownloadScore.Set(cb, OnDownloadScore);
                return true;
            }
            return false;
        }

        private void OnFindLeaderboardUploadScore(LeaderboardFindResult_t r, bool failure)
        {
            // no operations can be done if the target leaderboard has not been found, so this requires a flag
            foundLeaderboard = true;
            if (r.m_bLeaderboardFound == 1 && !failure)
            {
                Debug.Log("Found leaderboard.");
                currentLeaderboard = r.m_hSteamLeaderboard;
                UploadTimeBestTime(newScore);
            }
            else
                Debug.Log("Failed to find leaderboard.");
        }

        private void OnUploadScore(LeaderboardScoreUploaded_t r, bool failure)
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

        private void OnDownloadScore(LeaderboardScoresDownloaded_t r, bool failure)
        {
            if (r.m_cEntryCount > 0 && !failure)
            {
                Debug.Log("Scores downloaded.");
                readableLeaderboard.Clear();
                for (int i = 0; i < r.m_cEntryCount; ++i)
                {
                    LeaderboardEntry_t tempEntry;
                    if (!SteamUserStats.GetDownloadedLeaderboardEntry(r.m_hSteamLeaderboardEntries, i, out tempEntry, null, 0))
                        continue;
                    LeaderboardEntry toAdd = new LeaderboardEntry
                    {
                        rank = tempEntry.m_nGlobalRank,
                        playername = SteamFriends.GetFriendPersonaName(tempEntry.m_steamIDUser),
                        timeSeconds = tempEntry.m_nScore / 1000f
                    };
                    readableLeaderboard.Add(toAdd);
                }
            }
            else
                Debug.Log("Failed to download scores.");
        }
    }
}
