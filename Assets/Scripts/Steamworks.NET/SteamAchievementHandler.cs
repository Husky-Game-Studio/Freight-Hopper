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

        void Start()
        {
            if (SteamManager.Initialized)
            {
                if (SteamUserStats.RequestCurrentStats())
                {
                    foreach (StatAchieveMap.AchievementStats a in mapSO.map)
                        stat_achievementMap[a.stat] = a.achievements.ToArray();

                }
            }
        }

        private bool UnlockAchievement(string id)
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

        // takes in a stat API name
        private bool ProgressStat(string id, int increase)
        {
            int progress;
            if (SteamUserStats.GetStat(id, out progress))
            {
                return SetAchievementProgress(id, progress + increase);
            }

            return false;
        }

        // takes in a stat API name
        private bool ProgressStat(string id, float increase)
        {
            float progress;
            if (SteamUserStats.GetStat(id, out progress))
            {
                return SetAchievementProgress(id, progress + increase);
            }
            return false;
        }

        private bool SetAchievementProgress(string id, int progress)
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
                            Debug.Log(SteamUserStats.SetAchievement(id));
                }
                
            }
            if (SteamUserStats.SetStat(id, progress))
                return SteamUserStats.StoreStats();
            return false;
        }

        private bool SetAchievementProgress(string id, float progress)
        {
            if (stat_achievementMap.ContainsKey(id))
            {
                foreach (string s in stat_achievementMap[id])
                {
                    float minProg;
                    float maxProg;
                    // technically would be more optimal to just set instead of get
                    // however, the get is necessary if I want to prioritize the steam interface
                    if (SteamUserStats.GetAchievementProgressLimits(s, out minProg, out maxProg))
                        if (progress >= maxProg)
                            Debug.Log(SteamUserStats.SetAchievement(id));
                }
                return SteamUserStats.StoreStats();
            }
            if (SteamUserStats.SetStat(id, progress))
                return SteamUserStats.StoreStats();
            return false;
        }

        private bool ClearAchievement(string id)
        {
            return SteamUserStats.ClearAchievement(id);
        }

        private bool ObliterateEverything()
        {
            return SteamUserStats.ResetAllStats(true);
        }
    }
}
