using System;
using UnityEngine;
using System.IO;
using System.Text;

public static class LevelTimeSaveLoader
{
    const int saveVersion = 1;
    const string levelDataPath = "/LevelTimes/";
    const string fileExtension = ".lvl";
    // Deletes all .lvl files in /LevelTimes/
    public static void ClearBestTimeData()
    {
        string folderpath = Application.persistentDataPath + levelDataPath;
        DirectoryInfo dir = new DirectoryInfo(folderpath);
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            if (file.Extension.Equals(fileExtension))
            {
                File.Delete(file.FullName);
            }
        }
    }

    public static void Save(string levelName, LevelTimeSaveData data)
    {
        string filename = GetFileName(levelName);
        Directory.CreateDirectory(Path.GetDirectoryName(filename) ?? throw new InvalidOperationException());
        using (var stream = File.Open(filename, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(saveVersion);
                writer.Write(data.BestTime);
                writer.Write(data.MedalIndex);
            }
        }
    }

    public static LevelTimeSaveData Load(string levelName)
    {
        LevelTimeSaveData temp = null;
        string filename = GetFileName(levelName);
        if (File.Exists(filename))
        {
            using (var stream = File.Open(filename, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    // File Version Number is an int
                    switch (reader.ReadInt32())
                    {
                        case 1:
                            temp = new LevelTimeSaveData(reader.ReadSingle(), levelName, false);
                            temp.SetNewMedalIndex(reader.ReadInt32(), false);
                            break;
                    }
                }
            }
        }
        return temp;
    }

    private static string GetFileName(string name)
    {
        StringBuilder sb = new StringBuilder(Application.persistentDataPath);
        sb.Append(levelDataPath);
        sb.Append(name);
        sb.Append(fileExtension);
        return sb.ToString();
    }
}