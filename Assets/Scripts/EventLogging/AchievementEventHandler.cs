using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementEventHandler : MonoBehaviour
{
    [SerializeField]
    private List<string> desertLevelNames = new List<string>();
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
            EventBoat.OnLevelComplete += CheckAndCountLevels;
        }
        else
            Debug.Log("Did not subscribe events because Steam Stats were not retrieved.");
    }

    // subscribble for level completes, which should increment the achievement count
    private void CheckAndCountLevels(LevelCompleteData level)
    {
        int levelCount = 0;
        int bronzeCount = 0;
        int silverCount = 0;
        int goldCount = 0;
        int robCount = 0;
        // this cleared levels count will be changed
        foreach (var s in desertLevelNames)
            if (LevelTimeSaveLoader.LevelSaveDataExists(s))
            {
                LevelSaveData save = LevelTimeSaveLoader.Load(s);
                if (save == null) continue; // likely parsing issue

                ++levelCount;
                if (save.MedalIndex >= 0)
                    ++bronzeCount;
                if (save.MedalIndex >= 1)
                    ++silverCount;
                if (save.MedalIndex >= 2)
                    ++goldCount;
                if (save.RobertoFound)
                    ++robCount;
            }       

        int currentProgress;
        if (SteamTrain.SteamAchievementHandler.GetStat(desertCompletionStat, out currentProgress))
            if (currentProgress <= levelCount)
                SteamTrain.SteamAchievementHandler.SetStatsAndCheckAchievements(desertCompletionStat, levelCount);
        if (SteamTrain.SteamAchievementHandler.GetStat(desertBronzeStat, out currentProgress))
            if (currentProgress <= bronzeCount)
                SteamTrain.SteamAchievementHandler.SetStatsAndCheckAchievements(desertBronzeStat, bronzeCount);
        if (SteamTrain.SteamAchievementHandler.GetStat(desertSilverStat, out currentProgress))
            if (currentProgress <= silverCount)
                SteamTrain.SteamAchievementHandler.SetStatsAndCheckAchievements(desertSilverStat, silverCount);
        if (SteamTrain.SteamAchievementHandler.GetStat(desertGoldStat, out currentProgress))
            if (currentProgress <= goldCount)
                SteamTrain.SteamAchievementHandler.SetStatsAndCheckAchievements(desertGoldStat, goldCount);
        if (SteamTrain.SteamAchievementHandler.GetStat(desertRobertoStat, out currentProgress))
            if (currentProgress <= robCount)
                SteamTrain.SteamAchievementHandler.SetStatsAndCheckAchievements(desertRobertoStat, robCount);

    }
}
