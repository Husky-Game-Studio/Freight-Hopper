[System.Serializable]
public class LevelSaveData
{
    public int Version = LevelTimeSaveLoader.SaveVersion;
    public int MedalIndex = -1;
    public bool RobertoFound = false;
    public string LevelName = string.Empty;

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
}