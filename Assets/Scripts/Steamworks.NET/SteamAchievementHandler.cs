using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Ore;

namespace SteamTrain
{    
    public class SteamAchievementHandler : OSingleton<SteamAchievementHandler>
    {
        [SerializeField]
        private StatAchieveMap mapSO; 
        public static Dictionary<string, string[]> stat_achievementMap = new Dictionary<string, string[]>();
        public static bool statsRetrieved { get; private set; } = false;
        public int testCase = 0;

        void Start()
        {
            if (SteamManager.Initialized)
            {
                statsRetrieved = SteamUserStats.RequestCurrentStats();
                if (statsRetrieved)
                    foreach (StatAchieveMap.AchievementStats a in mapSO.map)
                    {
                        stat_achievementMap[a.stat] = a.achievements.ToArray();
                        int man;
                        switch (testCase)
                        {
                            // give me half the progress of all the achievements
                            case 1:
                                GetStat(a.stat, out man);
                                Debug.Log(man);
                                Debug.Log("Set progress to 9 in all the stats.");
                                Debug.Log(SetStatsAndCheckAchievements(a.stat, 9));
                                break;
                            // give me all the progress of all the achievements
                            case 2:
                                GetStat(a.stat, out man);
                                Debug.Log(man);
                                Debug.Log("Set progress to 10 in all the stats.");
                                Debug.Log(SetStatsAndCheckAchievements(a.stat, 10));
                                break;
                            // give me the achievements
                            case 3:
                                Debug.Log("Unlock index one of all achievements.");
                                Debug.Log(UnlockAchievement(a.achievements[0]));
                                break;
                            // remove me the achievements
                            case 4:
                                Debug.Log("Delete index one of all achievements.");
                                Debug.Log(SteamUserStats.ClearAchievement(a.achievements[0]));
                                break;
                            case 5:
                                Debug.Log("No more achievements.");
                                Debug.Log(SteamUserStats.ResetAllStats(true));
                                break;
                        }
                    }
                else Debug.Log("Crippling depression");
            }
        }

        public static bool UnlockAchievement(string id)
        {
            bool flag;
            if(!SteamUserStats.GetAchievement(id, out flag))
            {
                Debug.Log($"Achivement {id} does not exist, or is not published, or the ID is wrong.");
            }
            if (!flag){
                if (SteamUserStats.SetAchievement(id))
                {
                    // this has a callback, which should probably be tracked somewhere for errors
                    return SteamUserStats.StoreStats();
                }
            }
               
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
                    if (progress >= 10){
                        if (UnlockAchievement(s)){
                            Debug.Log($"Unlocked achievement {s}");
                        }
                    } 
                }
            }
            if (SteamUserStats.SetStat(id, progress) && request)
                return SteamUserStats.StoreStats();
            Debug.Log($"{id} failed to set.");
            return false;
        }
    }
}
