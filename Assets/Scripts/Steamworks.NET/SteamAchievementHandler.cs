using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

namespace SteamTrain
{    
    public class SteamAchievementHandler : MonoBehaviour
    {
        [SerializeField]
        private StatAchieveMap mapSO; 
        public static Dictionary<string, string[]> stat_achievementMap = new Dictionary<string, string[]>();
        public static bool statsRetrieved { get; private set; } = false;

        void Start()
        {
            if (SteamManager.Initialized)
            {
                statsRetrieved = SteamUserStats.RequestCurrentStats();
                if (statsRetrieved)
                    foreach (StatAchieveMap.AchievementStats a in mapSO.map)
                        stat_achievementMap[a.stat] = a.achievements.ToArray();
            }
        }

        public static bool UnlockAchievement(string id)
        {
            bool flag;
            SteamUserStats.GetAchievement(id, out flag);
            if (!flag)
               if(SteamUserStats.SetAchievement(id))
                    // this has a callback, which should probably be tracked somewhere for errors
                   return SteamUserStats.StoreStats();

            Debug.Log("This achivement does not exist, or is not published.");
            return false;
        }

        public static bool GetStat(string id, out int stat)
        {
            return SteamUserStats.GetStat(id, out stat);
        }

        // takes in a stat API name
        public static bool ProgressStat(string id, int increase, bool request = true)
        {
            int progress;
            if (SteamUserStats.GetStat(id, out progress))
            {
                return SetStatsAndCheckAchievements(id, progress + increase, request);
            }

            return false;
        }

        public static void GetAllAchievementNames()
        {
            uint achievementCount = SteamUserStats.GetNumAchievements();
            for(uint i = 0; i < achievementCount; ++i)
            {
                Debug.Log(SteamUserStats.GetAchievementName(i));
            }
        }

        public static bool SetStatsAndCheckAchievements(string id, int progress, bool request = true)
        {
            if (stat_achievementMap.ContainsKey(id))
            {
                foreach (string s in stat_achievementMap[id])
                {
                    int minProg;
                    int maxProg;
                    // technically would be more optimal to just set instead of get
                    // however, the get is necessary if I want to prioritize the steam interface
                    if (SteamUserStats.GetAchievementProgressLimits(s, out minProg, out maxProg))
                        if (progress >= maxProg)
                            Debug.Log(UnlockAchievement(s));
                }
                
            }
            if (SteamUserStats.SetStat(id, progress) && request)
                return SteamUserStats.StoreStats();
            return false;
        }

        public static bool PushStatsToServer()
        {
            return SteamUserStats.StoreStats();
        }

        public static bool ClearAchievement(string id)
        {
            return SteamUserStats.ClearAchievement(id);
        }

        public static bool ObliterateEverything()
        {
            return SteamUserStats.ResetAllStats(true);
        }
    }
}
