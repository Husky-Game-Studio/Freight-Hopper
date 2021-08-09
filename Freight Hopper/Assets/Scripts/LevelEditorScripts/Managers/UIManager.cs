using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        //Sliders + Snap variables 
        public Slider xSlider = null;
        public Slider ySlider = null;
        public Slider zSlider = null;
        float sliderValue;
        float snapInterval = 10.0f;


        //Dropdown 
        public Dropdown transformDropdown = null;

        //Delete Button 
        public Button deleteButton = null; 

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

            ySlider.minValue = 0;
            ySlider.maxValue = 360;

            zSlider.minValue = -360;
            zSlider.maxValue = 360;

            xSlider.onValueChanged.AddListener(XValueChanged);
            ySlider.onValueChanged.AddListener(YValueChanged);
            zSlider.onValueChanged.AddListener(ZValueChanged);
            transformDropdown.onValueChanged.AddListener(ObjectSetUI);
            //deleteButton.onClick.AddListener(DeleteObject);
        }

        void Update()
        {
            MoveShape();
           // DeleteObject();

            if (mSelectedObject != null)
            {
                SetRotationText(transformDropdown.value);
                deleteButton.gameObject.SetActive(true);
            }
            else {
                deleteButton.gameObject.SetActive(false);
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
            xSlider.value = SliderSnap(p.x);
            ySlider.value = SliderSnap(p.y);
            zSlider.value = SliderSnap(p.z);
        }

        private void MoveShape()
        {
            if (mSelectedObject == null || transformDropdown.value == (int)Transform.Position) {

                target.transform.localPosition = new Vector3(xSlider.value, ySlider.value, zSlider.value);

                xSliderText.text = "X: " + SliderSnap(xSlider.value);
                ySliderText.text = "Y: " + SliderSnap(ySlider.value);
                zSliderText.text = "Z: " + SliderSnap(zSlider.value);

            }
            
        }

        private void SetRotationText(int dropdownValue) {

            if (dropdownValue == 1) {

                xSliderText.text = "X: " + SliderSnap(mSelectedObject.transform.parent.localRotation.eulerAngles.x);
                ySliderText.text = "Y: " + SliderSnap(mSelectedObject.transform.parent.localRotation.eulerAngles.y);
                zSliderText.text = "Z: " + SliderSnap(mSelectedObject.transform.parent.localRotation.eulerAngles.z);
            }
        
        } 

        private Vector3 ReadObjectXfrom()
        {
            Vector3 p;

            if (mSelectedObject != null && mSelectedObject.transform.parent != null && transformDropdown.value == (int)Transform.Position)
            {
                p = mSelectedObject.transform.parent.localPosition;

            }
            else if (mSelectedObject != null && transformDropdown.value == (int)Transform.Position) {

                p = mSelectedObject.transform.position;

            }
            else if (mSelectedObject != null && transformDropdown.value == (int)Transform.Rotation)
            {

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

            if (transformDropdown.value == (int)Transform.Position && mSelectedObject.transform.parent != null)
            {
                ySlider.minValue = 0;
                mSelectedObject.transform.parent.localPosition = p;
            }
            else if (transformDropdown.value == (int)Transform.Position) {
                ySlider.minValue = 0;
                mSelectedObject.transform.position = p;
            }
            else if (transformDropdown.value == (int)Transform.Rotation) {
                ySlider.minValue = -360;
                Quaternion objectRot = new Quaternion();
                objectRot.eulerAngles = p;
                mSelectedObject.transform.parent.localRotation = objectRot;

            }
            

        }

        void XValueChanged(float v)
        {
            Vector3 p = ReadObjectXfrom();
            p.x = SliderSnap(v);
            UISetObjectXform(ref p);
        }

        void YValueChanged(float v)
        {
            Vector3 p = ReadObjectXfrom();
            p.y = SliderSnap(v);
            UISetObjectXform(ref p);
        }

        void ZValueChanged(float v)
        {
            Vector3 p = ReadObjectXfrom();
            p.z = SliderSnap(v);
            UISetObjectXform(ref p);
        }

        public void DeleteObject() {

            if (mSelectedObject != null && mSelectedObject.transform.parent != null)
            {

                Destroy(mSelectedObject.transform.parent.gameObject);
            }
            else if (mSelectedObject != null) {

                Destroy(mSelectedObject.gameObject);
            }
        
        }

        private float SliderSnap(float sliderFloat) {

            sliderValue = sliderFloat;
            sliderValue = Mathf.Round(sliderValue / snapInterval) * snapInterval;

            sliderFloat = sliderValue;
            return sliderFloat;
        }
    }
}

