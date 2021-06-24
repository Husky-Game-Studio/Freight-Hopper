using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HGSLevelEditor
{
    public class LevelCreator : MonoBehaviour
    {
        //Variables for placing objects 
        GameObject objToPlace;

        public void PassObject(string ID) {

            objToPlace = ObjectManager.GetInstance().GetObject(ID).objPrefab;
        
        } 

    }
}
