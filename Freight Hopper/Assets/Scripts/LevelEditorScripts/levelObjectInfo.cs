using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HGSLevelEditor
{
    [System.Serializable]
    public class LevelObjectInfo : MonoBehaviour
    {
        //Need to ID the object somehow to reference later
        public string objectID;
        public int posX;
        public int posY;
        public int posZ;

        public float rotX;
        public float rotY;
        public float rotZ;

        //public SaveObjectInfo info; 

        //Might just need this variable instead -- if this is still commented, shh no it isn't 
        public Vector3 worldRotation; 

        private void Update()
        {

            posX = Mathf.RoundToInt(this.transform.position.x);
            posY = Mathf.RoundToInt(this.transform.position.y);
            posZ = Mathf.RoundToInt(this.transform.position.z);

   
            worldRotation = transform.localEulerAngles;

            rotX = worldRotation.x;
            rotY = worldRotation.y;
            rotZ = worldRotation.z;

        }

        //For Serialization 
        public LevelObjectInfo GetObject()
        {

            LevelObjectInfo savedObj = new LevelObjectInfo();
            savedObj.objectID = objectID;
            savedObj.posX = posX;
            savedObj.posY = posY;
            savedObj.posZ = posZ;

            savedObj.rotX = rotX;
            savedObj.rotY = rotY;
            savedObj.rotZ = rotZ;


            return savedObj;

        }

        //[System.Serializable]
        // public class SaveObjectInfo
        //{
        //    public string objID;
        //    public int Xpos;
        //    public int Ypos;
        //    public int Zpos;

        //    public float Xrot;
        //    public float Yrot;
        //    public float Zrot;




        //} 
    }
}
