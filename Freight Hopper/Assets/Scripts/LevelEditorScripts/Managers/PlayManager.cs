using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HGSLevelEditor
{
    public class PlayManager : MonoBehaviour
    {
        public string levelNamePlay;
        // Start is called before the first frame update
        void Start()
        {
            levelNamePlay = LevelManager.levelNameLoad;
            SaveLoadLevel.GetInstance().LoadButton(levelNamePlay);
        }

    }
}
