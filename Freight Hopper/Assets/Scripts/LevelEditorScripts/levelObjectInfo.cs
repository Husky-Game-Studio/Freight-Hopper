using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HGSLevelEditor {

    public class levelObjectInfo : MonoBehaviour
    {
        //Need to ID the object somehow to reference later -- need to make a manager 
        public string objectID;
        public int posX;
        public int posZ;

        //Might just need this variable instead 
        public Vector3 worldPosition; 


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        [System.Serializable]
        public class saveObjectInfo
        {



        }
    }
}










