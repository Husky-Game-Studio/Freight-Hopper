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
        SteamLeaderboard_t m_CurrentLeaderboard;

        //int m_nLeaderboardEntries; // How many entries do we have?
        //LeaderboardEntry_t m_leaderboardEntries[10]; // The entries


        private CallResult<LeaderboardFindResult_t> m_callResultFindLeaderboard = new CallResult<LeaderboardFindResult_t>();
        private CallResult<LeaderboardScoreUploaded_t> m_callResultUploadScore = new CallResult<LeaderboardScoreUploaded_t>();
        private CallResult<LeaderboardScoresDownloaded_t> m_callResultDownloadScore = new CallResult<LeaderboardScoresDownloaded_t>();

        private void Start()
        {
            FindLeaderboard("1 1");
        }

        public void FindLeaderboard(string name)
        {
            SteamAPICall_t cb = SteamUserStats.FindLeaderboard(name);
            m_callResultFindLeaderboard.Set(cb, OnFindLeaderboard);
        }
        /*public bool UploadScore(int score)
        {

        }
        public bool DownloadScores()
        {

        }*/

        public void OnFindLeaderboard(LeaderboardFindResult_t r, bool failure)
        {
            Debug.Log(r.m_bLeaderboardFound);
            if(r.m_bLeaderboardFound == 1 && !failure)
            {
                m_CurrentLeaderboard = r.m_hSteamLeaderboard;
                SteamAPICall_t cb = SteamUserStats.UploadLeaderboardScore(m_CurrentLeaderboard,
                                                                          ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
                                                                          40000, null, 0);
                m_callResultUploadScore.Set(cb, OnUploadScore);

            }
        }
        public void OnUploadScore(LeaderboardScoreUploaded_t r, bool bIOFailure)
        {
            Debug.Log(r.m_bSuccess);
        }
        public void OnDownloadScore(LeaderboardScoresDownloaded_t r, bool bIOFailure)
        {

        }

    }
}
