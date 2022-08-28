using UnityEngine;

[System.Serializable]
public class LevelTimeSaveData
{
    [SerializeField] private float bestTime = float.PositiveInfinity;
    [SerializeField] private int medalIndex = -1;
    string levelName;
    public float BestTime => bestTime;
    public int MedalIndex => medalIndex;

    public void SetNewMedalIndex(int value, bool save = true)
    {
        if (medalIndex == value)
        {
            return;
        }
        medalIndex = value;
        if (save)
        {
            LevelTimeSaveLoader.Save(levelName, this);
        }
    }

    public LevelTimeSaveData(float bestTime, string levelName, bool save = true)
    {
        this.levelName = levelName;
        if (save)
        {
            SetNewBestTime(bestTime);
        }
        else
        {
            this.bestTime = bestTime;
        }
    }

    // returns the best time too, checks if time provided is the new best time first
    public float SetNewBestTime(float time)
    {
        if (time < bestTime)
        {
            bestTime = time;
            LevelTimeSaveLoader.Save(levelName, this);
            Debug.Log("saving new best time of " + bestTime);
        }
        return bestTime;
    }
}