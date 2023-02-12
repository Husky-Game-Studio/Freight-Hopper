using System.Collections.Generic;
using System;
using UnityEngine;

namespace SteamTrain
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Generic/Stat_Achievement_Map"), System.Serializable]
    public class StatAchieveMap : ScriptableObject
    {
        [System.Serializable]
        public class AchievementStats
        {
            public string stat;
            public List<string> achievements;
        }

        public List<AchievementStats> map;
    }
}
