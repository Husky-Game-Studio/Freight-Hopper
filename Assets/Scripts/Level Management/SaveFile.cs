using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ore;
using SteamTrain;
using UnityEngine;

public class SaveFile : OSingleton<SaveFile>
{
    public const int CacheVersion = 1;
    const string FILEPATH = "/data.cache";

    bool isDirty;
    string filepath;
    Dictionary<string, object> objects = new Dictionary<string, object>();

    byte[] totallySecureKey = new byte[] // nooo stop :( there is nothing cool in this
    {
        0x00, 0x01,
        0x32, 0x53,
        0x04, 0xF5,
        0x06, 0x77,
        0x08, 0x09,
        0x3A, 0x0B,
        0x0C, 0x6D,
        0x0E, 0xAF
    };
    
    public string Filepath => filepath;
    bool usingIncorrectFilepath = false;
    FileInfo filePathInfoCache;
    string steamIDKey = "steamID";
    public ulong CachedSteamID()
    {
        if (SteamManager.Initialized)
        {
            ulong val = SteamManager.GetMySteamID();
            PlayerPrefs.SetString(steamIDKey, val.ToString());
            return val;
        }
        else
        {
            string result = PlayerPrefs.GetString(steamIDKey, string.Empty);
            if (result == string.Empty)
            {
                return 0;
            }
            return ulong.Parse(result);
        }
    }
    
    void Start()
    {
        LoadCacheFile();
    }
    
    void LoadCacheFile()
    {
        ulong id = CachedSteamID();
        usingIncorrectFilepath = id == 0;
        if (usingIncorrectFilepath)
        {
            filepath = string.Concat(Application.persistentDataPath, FILEPATH);
        }
        else
        {
            filepath = string.Concat(Application.persistentDataPath, "/", id, FILEPATH);
        }
        filePathInfoCache = new FileInfo(filepath);
        if (Filesystem.TryReadText(filepath, out string text))
        {
            string jsonText = Decrypt(text);
#if UNITY_EDITOR
            Debug.Log("Cache Read\n" + jsonText);
#endif
            objects = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonText);
        }
        else
        {
            if (!File.Exists(filepath))
            {
                objects = new Dictionary<string, object>();
                objects["CacheVersion"] = CacheVersion;
                WriteCacheFile();
            }
            else
            {
                objects = new Dictionary<string, object>();
                objects["CacheVersion"] = CacheVersion;
                Filesystem.LogLastException();
            }
        }
    }
    void WriteCacheFile()
    {
        var settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Include };
        var jsonSerializer = JsonSerializer.CreateDefault(settings); // CRINGE
        string json = JsonAuthority.Serialize(objects, jsonSerializer);
        string finalText = Encrypt(json);
        if (!Filesystem.TryWriteText(filepath, finalText))
        {
            Filesystem.LogLastException();
            return;
        }
    }
    
    bool CheckFileDirtied()
    {
        if (isDirty) return true;
        if (!filePathInfoCache.Exists)
        {
            checkedLeaderboard = false;
            return true;
        }
        return false;
    }

    public void FixedUpdate()
    {
        CheckWriteCacheFile();
    }

    void CheckWriteCacheFile()
    {
        if (usingIncorrectFilepath)
        {
            ulong id = CachedSteamID();
            usingIncorrectFilepath = id == 0;
            if (!usingIncorrectFilepath) LoadCacheFile();
        }
        if (!CheckFileDirtied()) return;
        
        WriteCacheFile();
        isDirty = false;
    }
    
    void OnApplicationQuit()
    {
        CheckWriteCacheFile();
    }

    #region Freight Hopper Specific
    // Level version string is in the format level-world-levelversion-playercontrollerversion
    // e.g. 1-1-1-1 means level 1, world 1 (desert), 1st level version, player controller version 1
    [CanBeNull]
    public LevelVersionData ReadLevelVersionData(string levelVersion)
    {
        return ReadValue<LevelVersionData>(levelVersion);
    }
    public void WriteLevelVersionData(string levelVersion, LevelVersionData versionData)
    {
        WriteValue(levelVersion, versionData);
    }

    public void ClearLevelVersionData()
    {
        RemoveAll<LevelVersionData>();
    }
    
    [CanBeNull]
    public LevelAchievementData ReadLevelAchievementData(string levelName)
    {
        return ReadValue<LevelAchievementData>(levelName);
    }
    public void WriteLevelAchievementData(string levelVersion, LevelAchievementData versionData)
    {
        WriteValue(levelVersion, versionData);
    }
    // Clear it all
    public void ClearAchivementData()
    {
        RemoveAll<LevelAchievementData>();
    }

    public void ClearAchivementData(LevelAchievementData.Type type)
    {
        List<(string, LevelAchievementData)> data = GetAllPairs<LevelAchievementData>();
        switch (type)
        {
            case LevelAchievementData.Type.RobertoFound:
                foreach (var val in data) val.Item2.RobertoFound = false;
                break;
            case LevelAchievementData.Type.BestMedalIndex:
                foreach (var val in data) val.Item2.MedalIndex = -1;
                break;
        }
        WriteAll(data);
    }

    static bool checkedLeaderboard = false;
    // Optional parameter for setting time locally
    public static IEnumerator GetMyUserTimeCached(string versionedLevelName, LeaderboardEntry result, float newTime = 24*60)
    {
        if (!checkedLeaderboard || Current.ReadLevelVersionData(versionedLevelName) == null)
        {
            yield return LeaderboardEventHandler.GetMyUserTime(versionedLevelName, result);
            if (result == null)
            {
                yield break;
            }
            Current.WriteLevelVersionData(versionedLevelName, new LevelVersionData(result.timeSeconds));
            checkedLeaderboard = true;
        }
        else
        {
            LevelVersionData data = Current.ReadLevelVersionData(versionedLevelName);
            if (data.Time > newTime)
            {
                data.Time = newTime;
                Current.WriteLevelVersionData(versionedLevelName, new LevelVersionData(newTime));
            }
            result.timeSeconds = data.Time;
        }
        
    }
    #endregion

    [CanBeNull]
    public T ReadValue<T>(string key)
    {
        if (objects.TryGetValue(key, out object value))
        {
            if (value is T data)
            {
                return data;
            }

            try
            {
                JObject j = (JObject)value;
                T ret = j.ToObject<T>();
                if (ret != null)
                {
                    objects[key] = ret;
                }
                return ret;
            }
            catch (InvalidCastException)
            {
                Debug.LogError($"{typeof(T)} can't be casted to object from key {key} as it is type {value.GetType()}");
                return default;
            }
        }
        return default;
    }
    public void WriteAll<T>(List<(string, T)> keyValueList)
    {
        foreach (var val in keyValueList)
        {
            objects[val.Item1] = val.Item2;
        }
        isDirty = true;
    }
    public void RemoveAll<T>()
    {
        List<(string, T)> keys = GetAllPairs<T>();
        foreach ((string, T) key in keys)
        {
            objects.Remove(key.Item1);
        }
        isDirty = true;
    }
    public List<(string, T)> GetAllPairs<T>()
    {
        List<(string, T)> keys = new List<(string, T)>();
        foreach (var pair in objects)
        {
            if (pair.Value is T)
            {
                keys.Add((pair.Key, (T)pair.Value));
            }
        }
        return keys;
    }
    public List<T> GetAll<T>()
    {
        List<T> keys = new List<T>();
        foreach (var pair in objects)
        {
            if (pair.Value is T)
            {
                keys.Add((T)pair.Value);
            }
        }
        return keys;
    }

    public string CacheFileAsString()
    {
        if (Filesystem.TryReadText(filepath, out string text))
        {
#if UNITY_EDITOR
            Debug.Log("Cache Read\n" + Decrypt(text));
#endif
            return Decrypt(text);
        }
        return "";
    }
    
    public void WriteValue<T>(string key, T val)
    {
        objects[key] = val;
        isDirty = true;
    }
    public string Encrypt(string plainText)
    {
        string encrypted;

        using (Aes aes = Aes.Create())
        {
            aes.Key = totallySecureKey;
            aes.GenerateIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = Convert.ToBase64String(aes.IV.Concat(msEncrypt.ToArray()).ToArray());
                }
            }
        }

        return encrypted;
    }
    public string Decrypt(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        string plaintext = "";

        using (Aes aes = Aes.Create())
        {
            aes.Key = totallySecureKey;
            aes.IV = cipherBytes.Take(16).ToArray();

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes.Skip(16).ToArray()))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }
}
public class LevelAchievementData
{
    public bool RobertoFound;
    public int MedalIndex;
    
    public enum Type
    {
        RobertoFound = 0,
        BestMedalIndex = 1
    }
}

public class LevelVersionData
{
    public float Time { get; set; }

    public LevelVersionData(float time)
    {
        Time = time;
    }
}
