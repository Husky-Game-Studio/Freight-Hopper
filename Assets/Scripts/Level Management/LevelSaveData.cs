using UnityEngine;

[System.Serializable]
public class LevelSaveData
{
    public int Version = LevelTimeSaveLoader.SaveVersion;
    public float BestTime = float.PositiveInfinity;
    public int MedalIndex = -1;
    public bool RobertoFound = false;
    public string LevelName = string.Empty;

    // returns the best time too, checks if time provided is the new best time first
    public (float Time, bool IsNew) SetNewBestTime(float time, bool save = false)
    {
        bool isNew = time < BestTime || save;
        if (isNew)
        {
            BestTime = time;
            LevelTimeSaveLoader.Save(LevelName, this);
            Debug.Log("saving new best time of " + BestTime);
        }
        return (BestTime, isNew);
    }

    public void SetNewMedalIndex(int value, bool save = true)
    {
        if (MedalIndex == value)
        {
            return;
        }
        MedalIndex = value;
        if (save)
        {
            LevelTimeSaveLoader.Save(LevelName, this);
        }
    }
    public void ResetBestTime(){
        BestTime = float.PositiveInfinity;
    }
}