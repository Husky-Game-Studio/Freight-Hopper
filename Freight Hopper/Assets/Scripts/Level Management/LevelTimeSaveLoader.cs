using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

public static class LevelTimeSaveLoader
{
    public static void Save(string levelName, LevelTimeSaveData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string filename = GetFileName(levelName);
        Directory.CreateDirectory(Path.GetDirectoryName(filename));
        FileStream file = File.Create(filename);
        bf.Serialize(file, data);
        file.Close();
    }

    public static LevelTimeSaveData Load(string levelName)
    {
        LevelTimeSaveData temp = null;
        string filename = GetFileName(levelName);
        if (File.Exists(filename))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filename, FileMode.Open);
            temp = (LevelTimeSaveData)bf.Deserialize(file);
            file.Close();
        }
        return temp;
    }

    private static string GetFileName(string name)
    {
        StringBuilder sb = new StringBuilder(Application.persistentDataPath);
        sb.Append("/LevelTimes/");
        sb.Append(name);
        sb.Append(".lvl");
        return sb.ToString();
    }
}