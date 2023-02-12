using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementEventHandler : MonoBehaviour
{
    [SerializeField]
    private List<string> desertLevelNamesForAchievements;
    [SerializeField]
    private string desertCompletionStat = "DesertCompletionStat";
    [SerializeField]
    private string desertBronzeStat = "DesertBronzeStat";
    [SerializeField]
    private string desertSilverStat = "DesertSilverStat";
    [SerializeField]
    private string desertGoldStat = "DesertGoldStat";
    [SerializeField]
    private string desertRobertoStat = "DesertRobertoStat";

    // Start is called before the first frame update
    void Start()
    {
        if (SteamTrain.SteamAchievementHandler.statsRetrieved)
        {
            EventBoat.AddLevelComplete += CheckAndCountLevels;
            EventBoat.BronzeMedalUnlock += CheckAndCountBronzeMedals;
            EventBoat.SilverMedalUnlock += CheckAndCountSilverMedals;
            EventBoat.GoldMedalUnlock += CheckAndCountGoldMedals;
            EventBoat.SeenRoberto += CheckAndCountRobertos;
        }
    }

    // subscribble for level completes, which should increment the achievement count
    private void CheckAndCountLevels(string level)
    {
        int count = 0;
        // this cleared levels count will be changed
        foreach (string s in desertLevelNamesForAchievements)
        {
            if (LevelTimeSaveLoader.Load(level) != null)
                ++count;
        }

        int currentProgress;
        if (SteamTrain.SteamAchievementHandler.GetStat(desertCompletionStat, out currentProgress))
            if (currentProgress < count)
                SteamTrain.SteamAchievementHandler.SetStatsAndCheckAchievements(desertCompletionStat, count);
    }

    private void CheckAndCountBronzeMedals(string level)
    {
        int count = 0;
        // this cleared levels count will be changed
        foreach (string s in desertLevelNamesForAchievements)
        {
            LevelTimeSaveData save = LevelTimeSaveLoader.Load(level);
            if (save != null)
                if (save.MedalIndex >= 0)
                    ++count;
        }

        int currentProgress;
        if (SteamTrain.SteamAchievementHandler.GetStat(desertBronzeStat, out currentProgress))
            if (currentProgress < count)
                SteamTrain.SteamAchievementHandler.SetStatsAndCheckAchievements(desertBronzeStat, count);
    }

    private void CheckAndCountSilverMedals(string level)
    {
        int count = 0;
        // this cleared levels count will be changed
        foreach (string s in desertLevelNamesForAchievements)
        {
            LevelTimeSaveData save = LevelTimeSaveLoader.Load(level);
            if (save != null)
                if (save.MedalIndex >= 1)
                    ++count;
        }

        int currentProgress;
        if (SteamTrain.SteamAchievementHandler.GetStat(desertSilverStat, out currentProgress))
            if (currentProgress < count)
                SteamTrain.SteamAchievementHandler.SetStatsAndCheckAchievements(desertSilverStat, count);
    }

    private void CheckAndCountGoldMedals(string level)
    {
        int count = 0;
        // this cleared levels count will be changed
        foreach (string s in desertLevelNamesForAchievements)
        {
            LevelTimeSaveData save = LevelTimeSaveLoader.Load(level);
            if (save != null)
                if (save.MedalIndex >= 2)
                    ++count;
        }

        int currentProgress;
        if (SteamTrain.SteamAchievementHandler.GetStat(desertSilverStat, out currentProgress))
            if (currentProgress < count)
                SteamTrain.SteamAchievementHandler.SetStatsAndCheckAchievements(desertGoldStat, count);
    }

    private void CheckAndCountRobertos(string level)
    {
        int count = 0;
        // this cleared levels count will be changed
        foreach (string s in desertLevelNamesForAchievements)
        {
            if (LevelTimeSaveLoader.Load(level) != null)
                ++count;
        }

        int currentProgress;
        if (SteamTrain.SteamAchievementHandler.GetStat(desertCompletionStat, out currentProgress))
            if (currentProgress < count)
                SteamTrain.SteamAchievementHandler.SetStatsAndCheckAchievements(desertRobertoStat, count);
    }
}
