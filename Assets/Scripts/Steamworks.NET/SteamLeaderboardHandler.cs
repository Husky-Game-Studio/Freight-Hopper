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
        private static SteamLeaderboard_t m_CurrentLeaderboard;
        public static bool foundLeaderboard { get; private set; } = false;

        private static CallResult<LeaderboardFindResult_t> m_callResultFindLeaderboard = new CallResult<LeaderboardFindResult_t>();
        private static CallResult<LeaderboardScoreUploaded_t> m_callResultUploadScore = new CallResult<LeaderboardScoreUploaded_t>();
        private static CallResult<LeaderboardScoresDownloaded_t> m_callResultDownloadScore = new CallResult<LeaderboardScoresDownloaded_t>();

        public static void FindLeaderboard(string name)
        {
            foundLeaderboard = false;
            SteamAPICall_t cb = SteamUserStats.FindLeaderboard(name);
            m_callResultFindLeaderboard.Set(cb, OnFindLeaderboard);
        }

        public static bool UploadTimeBestTime(float time)
        {
            if(foundLeaderboard)
            {
                // makes sure that it only keeps best
                SteamAPICall_t cb = SteamUserStats.UploadLeaderboardScore(m_CurrentLeaderboard,
                                                                          ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
                                                                          (int)time, null, 0);
                m_callResultUploadScore.Set(cb, OnUploadScore);
                return true;
            }
            return false;
        }

        public static bool UploadTimeForced(float time)
        {
            if (foundLeaderboard)
            {
                // makes sure that it forcibly updates
                SteamAPICall_t cb = SteamUserStats.UploadLeaderboardScore(m_CurrentLeaderboard,
                                                                          ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate,
                                                                          (int)time, null, 0);
                m_callResultUploadScore.Set(cb, OnUploadScore);
                return true;
            }
            return false;
        }

        public static bool DownloadScores()
        {
            if (foundLeaderboard)
            {
                SteamAPICall_t cb = SteamUserStats.DownloadLeaderboardEntries(m_CurrentLeaderboard,
                                                                          ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal,
                                                                          1, 10);
                m_callResultDownloadScore.Set(cb, OnDownloadScore);
                return true;
            }
            return false;
        }

        public static void OnFindLeaderboard(LeaderboardFindResult_t r, bool failure)
        {
            // no operations can be done if the target leaderboard has not been found, so this requires a flag
            foundLeaderboard = true;
            if (r.m_bLeaderboardFound == 1 && !failure)
            {
                Debug.Log("Found leaderboard.");
                m_CurrentLeaderboard = r.m_hSteamLeaderboard;
            }
            else
                Debug.Log("Failed to find leaderboard.");
        }

        public static void OnUploadScore(LeaderboardScoreUploaded_t r, bool failure)
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

        public static void OnDownloadScore(LeaderboardScoresDownloaded_t r, bool failure)
        {
            if (r.m_cEntryCount > 0 && !failure)
            {
                Debug.Log("Scores downloaded.");
            }
            else
                Debug.Log("Failed to download scores.");
        }

    }
}
