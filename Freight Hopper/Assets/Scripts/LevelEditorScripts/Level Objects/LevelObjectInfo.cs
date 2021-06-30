using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HGSLevelEditor
{
    [System.Serializable]
    public class LevelObjectInfo : MonoBehaviour
    {
        public Vector3 worldRotation;

        public LevelObjectData data;

        private void Update()
        {
            data.posX = Mathf.RoundToInt(this.transform.position.x);
            data.posY = Mathf.RoundToInt(this.transform.position.y);
            data.posZ = Mathf.RoundToInt(this.transform.position.z);


            worldRotation = transform.localEulerAngles;

            data.rotX = worldRotation.x;
            data.rotY = worldRotation.y;
            data.rotZ = worldRotation.z;

        }

        //For Serialization 
        public LevelObjectData GetObject()
        {
            LevelObjectData savedObj = data;

            return savedObj;

        }
        public string GetID() {

            LevelObjectData savedObj = data;
            return savedObj.objectID;
        }
    }
}
