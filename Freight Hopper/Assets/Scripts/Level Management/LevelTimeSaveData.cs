using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelTimeSaveData
{
    // This is all temp really, this isn't really that fast as we want
    [SerializeField] private float bestTime = float.NaN;
    [SerializeField] private int medalIndex = -1;
    public float BestTime => bestTime;
    public int MedalIndex => medalIndex;

    public void SetNewMedalIndex(int value)
    {
        medalIndex = value;
    }

    public LevelTimeSaveData(float bestTime)
    {
        this.bestTime = bestTime;
    }

    // returns the best time too, checks if time provided is the new best time first
    public float SetNewBestTime(float time)
    {
        bestTime = Mathf.Min(time, bestTime);
        return bestTime;
    }
}