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

[System.Serializable]
public class SaveLoadLevel : MonoBehaviour
{
    //This is for UI -- Need to be able to access file names for Loading Level Selection
    public List<String> allLevels = new List<string>();

    List<LevelObjectData> saveObjectInfoList = new List<LevelObjectData>();

    LevelManager levelM; 

    public static string saveFolderName = "Level";

    void Start()
    {
        levelM = LevelManager.GetInstance();

    }
    public void SaveLevelButton(string levelName) {

        
        SavingLevel(levelName);
        GridLoadLevelUI.GetInstance().ReloadLevels();
    }

    public void LoadButton(string levelName) {

        LoadingLevel(levelName);
    }

    void SavingLevel(string levelName) {

        //for (int i = 0; i < levelM.o.Count; i++) {

        //    levelM.o[i].obj.SetActive(true);
        
        //}
        
        LevelObjectInfo [] obj = FindObjectsOfType<LevelObjectInfo>();

        saveObjectInfoList.Clear();

        foreach (LevelObjectInfo levelObj in obj) {

            saveObjectInfoList.Add(levelObj.GetObject());
        
        }
    
        //Going to change to this later 
        //currentLevel = new Level(saveObjectInfoList, levelName);

        SaveLevel levelSave = new SaveLevel();
        levelSave.SaveObject_List = saveObjectInfoList;

        string saveLocation = SavingLocation(levelName);

        //Saving Level
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(saveLocation, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, levelSave);
        stream.Close();

        Debug.Log(saveLocation + "works");
    }

    bool LoadingLevel(string levelName) {

        bool retrieve = true;

        string fileName = SavingLocation(levelName);
        Debug.Log(fileName);

        if (!File.Exists(fileName))
        {
            Debug.Log("Cannot find level!");
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
        string savingLocation = Application.streamingAssetsPath + "/Levels/";

        if (!Directory.Exists(savingLocation))
        {

            Directory.CreateDirectory(savingLocation);

        }

      
        return savingLocation + levelName;
    }

    void LoadActualLevel(SaveLevel savedLevel) {

        LevelManager.GetInstance().ClearLevel();
    
        for (int i = 0; i < savedLevel.SaveObject_List.Count; i++) {
           
            //LevelObjectInfo s_obj = savedLevel.SaveObject_List[i];
            LevelObjectData s_obj_data = savedLevel.SaveObject_List[i];

            Debug.Log("savedLevel Count: " + savedLevel.SaveObject_List.Count);
            Debug.Log("i: " + i); 

            Vector3 pos; 
            pos.x = s_obj_data.posX;
            pos.y = s_obj_data.posY;
            pos.z = s_obj_data.posZ;


            Debug.Log("POS: " + pos.x + pos.y + pos.z);
            Debug.Log("Rotation: " + s_obj_data.rotX + s_obj_data.rotY + s_obj_data.rotZ);
            

            GameObject load = Instantiate(HGSLevelEditor.ObjectManager.GetInstance().GetObject(s_obj_data.objectID).objPrefab, pos, 
                Quaternion.Euler(s_obj_data.rotX, s_obj_data.rotY, s_obj_data.rotZ));

           
            //I don't think I need any of this !! yay !! I'm gonna keep it here juuuuust in case. 
            //load.AddComponent<LevelObjectInfo>()
            //Error here !! 
            //levelM.o[Mathf.RoundToInt(load.transform.position.y)].inSceneObjects.Add(load);
            //load.transform.parent = levelM.o[Mathf.RoundToInt(load.transform.position.y)].obj.transform;

        }
    }

    [Serializable]
    public class SaveLevel {

        [SerializeField]public List<LevelObjectData> SaveObject_List; 
    }

    public void LoadAllLevels() { 
    
        DirectoryInfo dirInfo = new DirectoryInfo(Application.streamingAssetsPath + "/Levels");
        FileInfo[] fileInfo = dirInfo.GetFiles();

        foreach (FileInfo file in fileInfo) {

            allLevels.Add(file.Name);
            Debug.Log(file.Name);

        }

    }

    public static SaveLoadLevel instance;
    public static SaveLoadLevel GetInstance() {

        return instance;
    }

    void Awake()
    {
        instance = this;
        LoadAllLevels(); 
    }
}
