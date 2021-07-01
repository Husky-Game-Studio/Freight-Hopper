using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Attached to the Play button -- will load the level that is currently in the editor 
namespace HGSLevelEditor
{
    public class PlayManager : MonoBehaviour
    {
        void Start()
        {
            SaveLoadLevel.GetInstance().LoadButton(LevelManager.levelNameLoad, false);
        }

    }
}
