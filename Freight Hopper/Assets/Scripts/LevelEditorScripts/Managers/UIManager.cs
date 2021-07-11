using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

namespace HGSLevelEditor
{
    public partial class UIManager : MonoBehaviour
    {
        public Camera MainCamera = null;
        private GameObject mSelectedObject = null;
        public GameObject target;
        public float holdXvalue;

        //Texts 
        public Text xSliderText = null;
        public Text ySliderText = null;
        public Text zSliderText = null;

        //Sliders 
        public UnityEngine.UI.Slider xSlider = null;
        public UnityEngine.UI.Slider ySlider = null;
        public UnityEngine.UI.Slider zSlider = null;

        //Dropdown 
        public Dropdown transformDropdown = null;

        private enum Transform
        {
            Position = 0,
            Rotation = 1
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Assert(MainCamera != null);


            xSlider.minValue = -360;
            xSlider.maxValue = 360;

            ySlider.minValue = -360;
            ySlider.maxValue = 360;

            zSlider.minValue = -360;
            zSlider.maxValue = 360;

            xSlider.onValueChanged.AddListener(XValueChanged);
            ySlider.onValueChanged.AddListener(YValueChanged);
            zSlider.onValueChanged.AddListener(ZValueChanged);
            transformDropdown.onValueChanged.AddListener(ObjectSetUI);
        }

        void Update()
        {
            MoveShape();
            DeleteObject();

            if (mSelectedObject != null) {
                SetRotationText(transformDropdown.value);
            }

        }

        public void SetSelectedObject(GameObject g)
        {
            mSelectedObject = g;
            ObjectSetUI(transformDropdown.value);
  
        }

        public void ObjectSetUI(int value)
        {
            Vector3 p = ReadObjectXfrom();
            xSlider.value = p.x;
            ySlider.value = p.y;
            zSlider.value = p.z;
        }

        private void MoveShape()
        {
            if (mSelectedObject == null || transformDropdown.value == (int)Transform.Position) {

                target.transform.localPosition = new Vector3(xSlider.value, ySlider.value, zSlider.value);

                xSliderText.text = "X: " + xSlider.value;
                ySliderText.text = "Y: " + ySlider.value;
                zSliderText.text = "Z: " + zSlider.value;

            }
            
        }

        private void SetRotationText(int dropdownValue) {

            if (dropdownValue == 1) {

                xSliderText.text = "X: " + mSelectedObject.transform.parent.localRotation.eulerAngles.x;
                ySliderText.text = "Y: " + mSelectedObject.transform.parent.localRotation.eulerAngles.y;
                zSliderText.text = "Z: " + mSelectedObject.transform.parent.localRotation.eulerAngles.z;
            }
        
        } 

        private Vector3 ReadObjectXfrom()
        {
            Vector3 p;

            if (mSelectedObject != null && transformDropdown.value == (int)Transform.Position)
            {
                p = mSelectedObject.transform.parent.localPosition;
            }
            else if (mSelectedObject != null && transformDropdown.value == (int)Transform.Rotation) {

                p = mSelectedObject.transform.parent.localRotation.eulerAngles;
            }
            else
            {
                p = Vector3.zero;
            }

            return p;
        }

        private void UISetObjectXform(ref Vector3 p)
        {
            if (mSelectedObject == null)
                return;

            if (transformDropdown.value == (int)Transform.Position)
            {
                mSelectedObject.transform.parent.localPosition = p;
            }
            else if (transformDropdown.value == (int)Transform.Rotation) {

                Quaternion objectRot = new Quaternion();
                objectRot.eulerAngles = p; 
                mSelectedObject.transform.parent.localRotation = objectRot;

            }
            

        }

        void XValueChanged(float v)
        {
            Vector3 p = ReadObjectXfrom();
            p.x = v;
            UISetObjectXform(ref p);
        }

        void YValueChanged(float v)
        {
            Vector3 p = ReadObjectXfrom();
            p.y = v;
            UISetObjectXform(ref p);
        }

        void ZValueChanged(float v)
        {
            Vector3 p = ReadObjectXfrom();
            p.z = v;
            UISetObjectXform(ref p);
        }

        private void DeleteObject() {

            if (Input.GetKeyDown(KeyCode.Delete) && mSelectedObject != null) {

                Destroy(mSelectedObject.transform.parent.gameObject);
            }
        
        }
    }
}

