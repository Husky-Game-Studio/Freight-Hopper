using System.Text;
using UnityEngine;

[System.Serializable]
public struct LevelName
{
    // Format
    // Name 0-0
    // Name representing the world series name
    // First number is the worldID
    // Second number is the levelID
    // world series hold multiple worlds, and universes hold multiple galaxys. Universes have unique names
    public string worldSeriesName;
    public string worldID;
    public string levelID;

    public LevelName(string sceneName)
    {
        string[] splitText = sceneName.Split('-');
        worldSeriesName = "";
        if (splitText.Length == 3)
        {
            worldSeriesName = splitText[0];
            worldID = splitText[1];
            levelID = splitText[2];
        }
        else if (splitText.Length == 2)
        {
            worldSeriesName = "";
            worldID = splitText[0];
            levelID = splitText[1];
        }
        else
        {
            worldSeriesName = "";
            worldID = "0";
            levelID = "0";
            Debug.LogWarning("Unknown scene level name format. Please use 'Name-0-0' format or '0-0' format");
        }
    }

    private string ConvertToString(string world, string level)
    {
        StringBuilder str = new StringBuilder(worldSeriesName);
        if (!string.IsNullOrEmpty(worldSeriesName))
        {
            str.Append("-");
        }
        str.Append(world);
        str.Append("-");
        str.Append(level);
        return str.ToString();
    }

    public string CurrentLevel()
    {
        return ConvertToString(worldID, levelID);
    }
    public string VersionedCurrentLevel(LevelData level)
    {
        return $"{PlayerController.MajorVersion}-{level.Version}-{ConvertToString(worldID, levelID)}";
    }
    public string VersionedCurrentLevel(int playerControllerVersion, int levelVersion)
    {
        return $"{playerControllerVersion}-{levelVersion}-{ConvertToString(worldID, levelID)}";
    }
    public string NextLevel()
    {
        return ConvertToString(worldID, (int.Parse(levelID) + 1).ToString());
    }

    public string NextWorld()
    {
        return ConvertToString((int.Parse(worldID) + 1).ToString(), "1");
    }
}