using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HGSLevelEditor;
using System;


//[System.Serializable]
//// Seperate level into a seperate file called Level.cs -- will do after the basic prototype is finished 
//public class Level
//{
//    public string levelName;
//    public List<LevelObjectInfo> saveObjectInfoList = new List<LevelObjectInfo>();


//    public Level(List<LevelObjectInfo> objects, string name)
//    {
//        saveObjectInfoList = objects;
//        levelName = name;
//    }

//    public List<LevelObjectInfo> GetInfoList() {


//        return saveObjectInfoList;
    
    
//    }
//}

//[System.Serializable]
public class SaveLoadLevel : MonoBehaviour
{
    //This is for UI -- Need to be able to access file names for Loading Level Selection
    public List<String> allLevels = new List<string>();

    public static string saveFolderName = "Level";
    List<LevelObjectInfo> saveObjectInfoList = new List<LevelObjectInfo>();

    public void SaveButton() {

        SavingLevel("test");
    }

    public void LoadButton() {

        LoadingLevel("test");
    }

    void SavingLevel(string levelName) {
        LevelObjectInfo[] obj = FindObjectsOfType<LevelObjectInfo>();
        

        foreach (LevelObjectInfo levelObj in obj) {

            saveObjectInfoList.Add(levelObj);
        
        }
    
        //Going to change to this later 
        //currentLevel = new Level(saveObjectInfoList, levelName);

        SaveLevel levelSave = new SaveLevel();
        string saveLocation = SavingLocation(levelName);

        //Saving Level
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(saveLocation, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, levelSave);
        stream.Close();

        Debug.Log(saveLocation);
    }

    bool LoadingLevel(string levelName) {

        bool retrieve = true;

        string fileName = SavingLocation(levelName);

        if (!File.Exists(levelName))
        {

            retrieve = false;

        }

        else {

            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(fileName, FileMode.Open);

            //Need to change this to work with Level class 
            SaveLevel save = (SaveLevel)formatter.Deserialize(stream);
            stream.Close();
            LoadActualLevel(save);
        
        }

        return retrieve; 
    }

    static string SavingLocation(string levelName) {

        //Creating new location
        string savingLocation = Application.persistentDataPath + "/" + saveFolderName + "/";

        if (!Directory.Exists(savingLocation))
        {

            Directory.CreateDirectory(savingLocation);

        }

        return savingLocation + levelName;

    }

    void LoadActualLevel(SaveLevel savedLevel) {
        for (int i = 0; i < savedLevel.SaveObject_List.Count; i++) {

        
            LevelObjectInfo s_obj = savedLevel.SaveObject_List[i];

            Vector3 pos; 
            pos.x = s_obj.posX;
            pos.y = s_obj.posY;
            pos.z = s_obj.posZ;


            GameObject load = Instantiate(HGSLevelEditor.ObjectManager.GetInstance().getObject(s_obj.objectID).objPrefab, pos, 
                Quaternion.Euler(s_obj.rotX, s_obj.rotY, s_obj.rotZ)) as GameObject;
            
        
        }
    }

    [Serializable]
    public class SaveLevel {

        public List<LevelObjectInfo> SaveObject_List; 
    }

    public void LoadAllLevels() { 
    
        DirectoryInfo dirInfo = new DirectoryInfo(Application.streamingAssetsPath + "/Levels");
        FileInfo[] fileInfo = dirInfo.GetFiles();

        foreach (FileInfo file in fileInfo) {

            string[] readLevel = file.Name.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);


        
        
        }

    }

    public static SaveLoadLevel instance;
    public static SaveLoadLevel getInstance() {

        return instance;
    }

    void Awake()
    {
        instance = this;
        LoadAllLevels(); 
    }
}
