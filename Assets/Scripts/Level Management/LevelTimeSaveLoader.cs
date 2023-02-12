using System;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;

public static class LevelTimeSaveLoader
{
    public const int SaveVersion = 2;
    public const string levelDataPath = "/LevelTimes/";
    public const string fileExtension = ".lvl";

    // Deletes all .lvl files in /LevelTimes/
    public static void ClearBestTimeData()
    {
        string folderpath = Application.persistentDataPath + levelDataPath;
        FileInfo[] files = new DirectoryInfo(folderpath).GetFiles();
        foreach (FileInfo file in files)
        {
            if (file.Extension.Equals(fileExtension))
            {
                LevelSaveData data = Load(file.Name);
                data.ResetBestTime();
                Save(file.Name, data);
            }
        }
    }

    public static void Save(string levelName, LevelSaveData data)
    {
        string filename = GetFileName(levelName);
        
        Directory.CreateDirectory(Path.GetDirectoryName(filename) ?? throw new InvalidOperationException());
        string res = JsonConvert.SerializeObject(data);
        if (!Ore.Filesystem.TryWriteText(filename, Encrypt(res, SaveVersion), Encoding.UTF8)){
            Ore.Filesystem.LogLastException();
        }
    }
    // If you just want to know, without loading since its faster
    public static bool LevelSaveDataExists(string levelName) => File.Exists(GetFileName(levelName));
    public static LevelSaveData Load(string levelName)
    {
        // Version 2 loading
        string filename = GetFileName(levelName);
        if (!File.Exists(filename))
        {
            return null;
        }

        LevelSaveData temp = Load2(levelName);
        if(temp == null){
            temp = Load1(levelName);
        }

        return temp;
    }

    private static LevelSaveData Load1(string levelName){
        LevelSaveData temp = null;
        string filename = GetFileName(levelName);

        using (var stream = File.Open(filename, FileMode.Open))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
                // File Version Number is an int
                switch (reader.ReadInt32())
                {
                    case 1:
                        temp = new LevelSaveData();
                        temp.BestTime = reader.ReadSingle();
                        temp.SetNewMedalIndex(reader.ReadInt32(), false);
                        break;
                }
            }
        }
        
        return temp;
    }
    private static LevelSaveData Load2(string levelName){
        LevelSaveData temp = null;
        string filename = GetFileName(levelName);
        if (!Ore.Filesystem.TryReadText(filename, out string text, Encoding.UTF8)){
            Ore.Filesystem.LogLastException();
            return temp;
        }
        text = Decrypt(text, SaveVersion);
        try 
        {
            temp = JsonConvert.DeserializeObject<LevelSaveData>(text);
        } catch {
            return temp;
        }

        if (temp.Version != SaveVersion)
        {
            return null;
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

    private static string Encrypt(string s, int version) // The point isn't really to make it impossible, just not plain text anymore lmao
    {
        if(version == 2){
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }
        
        return s;
    }
    private static string Decrypt(string s, int version)
    {
        string tmp;
        if (version == 2){
            try
            {
                tmp = Encoding.UTF8.GetString(System.Convert.FromBase64String(s));
            }
            catch
            {
                return s;
            }
            return tmp;
        }
        return s;
    }
}