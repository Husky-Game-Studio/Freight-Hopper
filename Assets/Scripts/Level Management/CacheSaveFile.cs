using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using Ore;
using UnityEngine;

public class CacheSaveFile : OSingleton<CacheSaveFile>
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
    
    void Awake()
    {
        filepath = string.Concat(Application.persistentDataPath, FILEPATH);
        LoadCacheFile();
    }
    
    void LoadCacheFile()
    {
        if (Filesystem.TryReadText(filepath, out string text))
        {
#if UNITY_EDITOR
            Debug.Log("Cache Read\n" + Decrypt(text));
#endif
            objects = JsonAuthority.GenericParse(Decrypt(text)) as Dictionary<string, object>;
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
        string json = JsonAuthority.Serialize(objects);
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
        if (!File.Exists(filepath)) return true;
        return false;
    }

    public void FixedUpdate()
    {
        if (!CheckFileDirtied()) return;
        
        WriteCacheFile();
        isDirty = false;
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
            case LevelAchievementData.Type.GottenRobert:
                foreach (var val in data) val.Item2.GottenRoberto = false;
                break;
            case LevelAchievementData.Type.BestMedalIndex:
                foreach (var val in data) val.Item2.BestMedalIndex = -1;
                break;
        }
        WriteAll(data);
    }
    
    public class LevelVersionData
    {
        public float Time;
        public int MedalIndex;
    }

    public class LevelAchievementData
    {
        public enum Type
        {
            GottenRobert = 0,
            BestMedalIndex = 1
        }
        public bool GottenRoberto = false;
        public int BestMedalIndex = -1;
    }
    
    #endregion

    [CanBeNull]
    public T ReadValue<T>(string key)
    {
        if (objects.TryGetValue(key, out object value))
        {
            T data = (T)value;
            return data;
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
