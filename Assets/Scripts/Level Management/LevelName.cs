using System.Text;
using UnityEngine;

[System.Serializable]
public struct LevelName
{
    // Format
    // Name 0 0
    // Name representing the world series name
    // First number is the worldID
    // Second number is the levelID
    // world series hold multiple worlds, and universes hold multiple galaxys. Universes have unique names
    public string worldSeriesName;

    public int worldID;
    public int levelID;

    public LevelName(string sceneName)
    {
        string[] splitText = sceneName.Split(' ');
        worldSeriesName = "";
        if (splitText.Length == 3)
        {
            worldSeriesName = splitText[0];
            worldID = int.Parse(splitText[1]);
            levelID = int.Parse(splitText[2]);
        }
        else if (splitText.Length == 2)
        {
            worldSeriesName = "";
            worldID = int.Parse(splitText[0]);
            levelID = int.Parse(splitText[1]);
        }
        else
        {
            worldSeriesName = "";
            worldID = 0;
            levelID = 0;
            Debug.LogWarning("Unknown scene level name format. Please use 'Name 0 0' format or '0 0' format");
        }
    }

    private string ConvertToString(int world, int level)
    {
        StringBuilder str = new StringBuilder(worldSeriesName);
        if (!string.IsNullOrEmpty(worldSeriesName))
        {
            str.Append(" ");
        }
        str.Append(world);
        str.Append(" ");
        str.Append(level);
        return str.ToString();
    }

    public string CurrentLevel()
    {
        return ConvertToString(worldID, levelID);
    }

    public string NextLevel()
    {
        return ConvertToString(worldID, levelID + 1);
    }

    public string NextWorld()
    {
        return ConvertToString(worldID + 1, 1);
    }
}