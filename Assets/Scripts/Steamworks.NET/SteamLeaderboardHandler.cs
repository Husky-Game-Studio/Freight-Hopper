using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

namespace SteamTrain
{
    // https://stackoverflow.com/questions/37842524/how-to-do-leaderboards-in-steamworks-net
    // https://partner.steamgames.com/doc/features/leaderboards/guide
    public class SteamLeaderboardHandler : MonoBehaviour
    {
        public SteamLeaderboard_t currentLeaderboard { get; private set; }
        private CallResult<LeaderboardFindResult_t> callResultFindLeaderboard = new CallResult<LeaderboardFindResult_t>();
        private CallResult<LeaderboardScoreUploaded_t> callResultUploadScore = new CallResult<LeaderboardScoreUploaded_t>();
        private CallResult<LeaderboardScoresDownloaded_t> callResultDownloadScore = new CallResult<LeaderboardScoresDownloaded_t>();

        public bool foundLeaderboard { get; private set; } = false;

        public void FindLeaderboard(string name)
        {
            SteamAPICall_t cb = SteamUserStats.FindLeaderboard(name);
            callResultFindLeaderboard.Set(cb, OnFindLeaderboard);
        }

        public bool UploadTimeBestTime(float time, CallResult<LeaderboardScoreUploaded_t>.APIDispatchDelegate del)
        {
            if(foundLeaderboard)
            {
                // makes sure that it only keeps best
                SteamAPICall_t cb = SteamUserStats.UploadLeaderboardScore(currentLeaderboard,
                                                                          ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
                                                                          (int)time, null, 0);
                callResultUploadScore.Set(cb, del);
                return true;
            }
            return false;
        }

        public bool UploadTimeForced(float time, CallResult<LeaderboardScoreUploaded_t>.APIDispatchDelegate del)
        {
            if (foundLeaderboard)
            {
                // makes sure that it forcibly updates
                SteamAPICall_t cb = SteamUserStats.UploadLeaderboardScore(currentLeaderboard,
                                                                          ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate,
                                                                          (int)time, null, 0);
                callResultUploadScore.Set(cb, del);
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
                callResultDownloadScore.Set(cb, del);
                return true;
            }
            return false;
        }

        private void OnFindLeaderboard(LeaderboardFindResult_t r, bool failure)
        {
            // no operations can be done if the target leaderboard has not been found, so this requires a flag
            foundLeaderboard = true;
            if (r.m_bLeaderboardFound == 1 && !failure)
            {
                Debug.Log("Found leaderboard.");
                currentLeaderboard = r.m_hSteamLeaderboard;
            }
            else
                Debug.Log("Failed to find leaderboard.");
        }
    }
}
