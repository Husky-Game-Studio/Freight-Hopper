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
        private CallResult<RemoteStorageFileWriteAsyncComplete_t> callResultUploadFileData = new CallResult<RemoteStorageFileWriteAsyncComplete_t>();
        private CallResult<RemoteStorageFileShareResult_t> callResultShareFileData = new CallResult<RemoteStorageFileShareResult_t>();
        private CallResult<LeaderboardUGCSet_t> callResultLeaderboardFileSave = new CallResult<LeaderboardUGCSet_t>();

        public bool findingLeaderboard { get; private set; } = false;
        public bool foundLeaderboard { get; private set; } = false;
        public bool downloadingLeaderboards { get; private set; } = false;
        public bool readLeaderboards { get; private set; } = false;

        public int newScore;
        public int firstLimit;
        public int secondLimit;

        public string leaderboardFName;
        public byte[] leaderboardData;
        public CSteamID whomst;

        public List<LeaderboardEntry> readableLeaderboard = new List<LeaderboardEntry>();

        public void FindLeaderboardAndUploadScore(string name)
        {
            foundLeaderboard = false;
            findingLeaderboard = true;
            SteamAPICall_t cb = SteamUserStats.FindLeaderboard(name);
            callResultFindLeaderboard.Set(cb, OnFindLeaderboardUploadScore);
        }

        public void FindLeaderboardAndUploadData(string name)
        {
            foundLeaderboard = false;
            findingLeaderboard = true;
            SteamAPICall_t cb = SteamUserStats.FindLeaderboard(name);
            callResultFindLeaderboard.Set(cb, OnFindLeaderboardUploadData);
        }

        public void FindLeaderboardAndDownloadScores(string name)
        {
            foundLeaderboard = false;
            findingLeaderboard = true;
            SteamAPICall_t cb = SteamUserStats.FindLeaderboard(name);
            callResultFindLeaderboard.Set(cb, OnFindLeaderboardDownloadScores);
        }

        public void FindLeaderboardAndDownloadSomeGuysScore(string name)
        {
            foundLeaderboard = false;
            findingLeaderboard = true;
            SteamAPICall_t cb = SteamUserStats.FindLeaderboard(name);
            callResultFindLeaderboard.Set(cb, OnFindLeaderboardDownloadSomeGuysScore);
        }    

        public void FindLeaderboardAndFriendScores(string name)
        {
            foundLeaderboard = false;
            findingLeaderboard = true;
            SteamAPICall_t cb = SteamUserStats.FindLeaderboard(name);
            callResultFindLeaderboard.Set(cb, OnFindLeaderboardDownloadFriendScores);
        }

        public void FindLeaderboardAndDownloadRelativeScores(string name)
        {
            foundLeaderboard = false;
            findingLeaderboard = true;
            SteamAPICall_t cb = SteamUserStats.FindLeaderboard(name);
            callResultFindLeaderboard.Set(cb, OnFindLeaderboardDownloadRelativeScores);
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

        public bool UploadDataBestData(int time)
        {
            if (foundLeaderboard)
            {
                // makes sure that it only keeps best
                SteamAPICall_t cb = SteamUserStats.UploadLeaderboardScore(currentLeaderboard,
                                                                          ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
                                                                          time, null, 0);
                callResultUploadScore.Set(cb, OnUploadScoreData);
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

        public bool DownloadScores(int min, int max)
        {
            if (foundLeaderboard)
            {
                readLeaderboards = false;
                downloadingLeaderboards = true;
                SteamAPICall_t cb = SteamUserStats.DownloadLeaderboardEntries(currentLeaderboard,
                                                                          ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal,
                                                                          min, max);
                callResultDownloadScore.Set(cb, OnDownloadScore);
                return true;
            }
            return false;
        }

        public bool DownloadSomeGuysScore(CSteamID whomst)
        {
            if (foundLeaderboard)
            {
                readLeaderboards = false;
                downloadingLeaderboards = true;
                SteamAPICall_t cb = SteamUserStats.DownloadLeaderboardEntriesForUsers(currentLeaderboard, 
                                                                                      new CSteamID[1]{ whomst },
                                                                                      1);
                callResultDownloadScore.Set(cb, OnDownloadScore);
                return true;
            }
            return false;
        }

        public bool DownloadFriendScores(int min, int max)
        {
            if (foundLeaderboard)
            {
                readLeaderboards = false;
                downloadingLeaderboards = true;
                SteamAPICall_t cb = SteamUserStats.DownloadLeaderboardEntries(currentLeaderboard,
                                                                          ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends,
                                                                          min, max);
                callResultDownloadScore.Set(cb, OnDownloadScore);
                return true;
            }
            return false;
        }

        public bool DownloadRelativeScores(int less, int greater)
        {
            if (foundLeaderboard)
            {
                readLeaderboards = false;
                downloadingLeaderboards = true;
                SteamAPICall_t cb = SteamUserStats.DownloadLeaderboardEntries(currentLeaderboard,
                                                                              ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser,
                                                                              -less, greater);
                callResultDownloadScore.Set(cb, OnDownloadScore);
                return true;
            }
            return false;
        }

        private void OnFindLeaderboardDownloadScores(LeaderboardFindResult_t r, bool failure)
        {
            // no operations can be done if the target leaderboard has not been found, so this requires a flag
            findingLeaderboard = false;
            if (r.m_bLeaderboardFound == 1 && !failure)
            {
                foundLeaderboard = true;
                currentLeaderboard = r.m_hSteamLeaderboard;
                DownloadScores(firstLimit, secondLimit);
            }
            else
                Debug.Log("Failed to find leaderboard.");
        }

        private void OnFindLeaderboardDownloadSomeGuysScore(LeaderboardFindResult_t r, bool failure)
        {
            // no operations can be done if the target leaderboard has not been found, so this requires a flag
            findingLeaderboard = false;
            if (r.m_bLeaderboardFound == 1 && !failure)
            {
                foundLeaderboard = true;
                currentLeaderboard = r.m_hSteamLeaderboard;
                DownloadSomeGuysScore(whomst);
            }
            else
                Debug.Log("Failed to find leaderboard.");
        }

        private void OnFindLeaderboardDownloadFriendScores(LeaderboardFindResult_t r, bool failure)
        {
            // no operations can be done if the target leaderboard has not been found, so this requires a flag
            findingLeaderboard = false;
            if (r.m_bLeaderboardFound == 1 && !failure)
            {
                foundLeaderboard = true;
                currentLeaderboard = r.m_hSteamLeaderboard;
                DownloadFriendScores(firstLimit, secondLimit);
            }
            else
                Debug.Log("Failed to find leaderboard.");
        }

        private void OnFindLeaderboardDownloadRelativeScores(LeaderboardFindResult_t r, bool failure)
        {
            // no operations can be done if the target leaderboard has not been found, so this requires a flag
            findingLeaderboard = false;
            if (r.m_bLeaderboardFound == 1 && !failure)
            {
                foundLeaderboard = true;
                currentLeaderboard = r.m_hSteamLeaderboard;
                DownloadRelativeScores(firstLimit, secondLimit);
            }
            else
                Debug.Log("Failed to find relative leaderboard.");
        }

        private void OnFindLeaderboardUploadScore(LeaderboardFindResult_t r, bool failure)
        {
            // no operations can be done if the target leaderboard has not been found, so this requires a flag
            findingLeaderboard = false;
            if (r.m_bLeaderboardFound == 1 && !failure)
            {
                foundLeaderboard = true;
                currentLeaderboard = r.m_hSteamLeaderboard;
                UploadTimeBestTime(newScore);
            }
            else
                Debug.Log("Failed to find leaderboard.");
        }

        private void OnFindLeaderboardUploadData(LeaderboardFindResult_t r, bool failure)
        {
            // no operations can be done if the target leaderboard has not been found, so this requires a flag
            findingLeaderboard = false;
            if (r.m_bLeaderboardFound == 1 && !failure)
            {
                foundLeaderboard = true;
                currentLeaderboard = r.m_hSteamLeaderboard;
                UploadDataBestData(newScore);
            }
            else
                Debug.Log("Failed to find leaderboard.");
        }

        private void OnUploadScore(LeaderboardScoreUploaded_t r, bool failure)
        {
            if (r.m_bSuccess == 1 && !failure)
            {
                SteamBus.OnTimeUploaded.Invoke();
                if (r.m_bScoreChanged == 1)
                {
                    SteamBus.OnNewBestTime.Invoke();
                }
            }
            else
                Debug.Log("Failed to upload score.");
        }

        private void OnUploadScoreData(LeaderboardScoreUploaded_t r, bool failure)
        {
            if (r.m_bSuccess == 1 && !failure)
            {
                SteamBus.OnTimeUploaded.Invoke();
                if (r.m_bScoreChanged == 1)
                {
                    SteamBus.OnNewBestTime.Invoke();
                    SteamAPICall_t cb =  SteamRemoteStorage.FileWriteAsync(leaderboardFName, leaderboardData, (uint)leaderboardData.Length);
                    if(cb == (SteamAPICall_t)0)
                    {
                        Debug.Log(leaderboardFName);
                        Debug.Log("Crippling sad");
                    }
                    callResultUploadFileData.Set(cb, OnUploadFileData);
                    Debug.Log("goomba");

                }
            }
            else
                Debug.Log("Failed to upload score.");
        }

        private void OnDownloadScore(LeaderboardScoresDownloaded_t r, bool failure)
        {
            downloadingLeaderboards = false;
            if (r.m_cEntryCount > 0 && !failure)
            {
                readableLeaderboard.Clear();
                for (int i = 0; i < r.m_cEntryCount; ++i)
                {
                    LeaderboardEntry_t tempEntry;
                    if (!SteamUserStats.GetDownloadedLeaderboardEntry(r.m_hSteamLeaderboardEntries, i, out tempEntry, null, 0))
                        continue;
                    LeaderboardEntry toAdd = new LeaderboardEntry
                    {
                        rank = tempEntry.m_nGlobalRank,
                        playerName = SteamFriends.GetFriendPersonaName(tempEntry.m_steamIDUser),
                        timeSeconds = tempEntry.m_nScore / 1000f,
                        steamID = tempEntry.m_steamIDUser.m_SteamID
                    };
                    readableLeaderboard.Add(toAdd);
                }
                readLeaderboards = true;
            }
            else if (r.m_cEntryCount == 0)
                Debug.Log("No users in leaderboard");
            else
                Debug.Log("Failed to download scores.");
        }
        
        private void OnUploadFileData(RemoteStorageFileWriteAsyncComplete_t r, bool failure)
        {
            if (r.m_eResult == EResult.k_EResultOK && !failure)
            {
                SteamAPICall_t cb = SteamRemoteStorage.FileShare(leaderboardFName);
                callResultShareFileData.Set(cb, OnShareFileData);
            }
            else
                Debug.Log("Failed to upload leaderboard file.");
        }

        private void OnShareFileData(RemoteStorageFileShareResult_t r, bool failure)
        {
            if (r.m_eResult == EResult.k_EResultOK && !failure)
            {
                SteamAPICall_t cb = SteamUserStats.AttachLeaderboardUGC(currentLeaderboard, r.m_hFile);
                callResultLeaderboardFileSave.Set(cb, OnLeaderboardSaveFile);
            }
            else
                Debug.Log("Failed to share leaderboard file.");
        }

        private void OnLeaderboardSaveFile(LeaderboardUGCSet_t r, bool failure)
        {
            if (r.m_eResult == EResult.k_EResultOK && !failure)
            {
                Debug.Log("Leaderboard Save Data Full Process Succ.");
            }
            else
                Debug.Log("Failed to attach leaderboard file to leaderboard.");
        }
    }
}
