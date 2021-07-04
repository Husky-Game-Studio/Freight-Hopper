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

        //Sliders 
        public Slider xSlider = null;
        public Slider ySlider = null;
        public Slider zSlider = null;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Assert(MainCamera != null);


            xSlider.minValue = -100;
            xSlider.maxValue = 100;

            ySlider.minValue = -100;
            ySlider.maxValue = 100;

            zSlider.minValue = -100;
            zSlider.maxValue = 100;

            xSlider.onValueChanged.AddListener(XValueChanged);
            ySlider.onValueChanged.AddListener(YValueChanged);
            zSlider.onValueChanged.AddListener(ZValueChanged);
        }

        // Update is called once per frame
        void Update()
        {
            moveShape();

        }

        public void SetSelectedObject(GameObject g)
        {
            mSelectedObject = g;
            ObjectSetUI();
        }

        public void ObjectSetUI()
        {
            Vector3 p = ReadObjectXfrom();
            xSlider.value = p.x;  // do not need to call back for this comes from the object
            ySlider.value = p.y;
            zSlider.value = p.z;
        }

        void moveShape()
        {
            target.transform.localPosition = new Vector3(xSlider.value, ySlider.value, zSlider.value);

            xSliderText.text = "X: " + xSlider.value;
            ySliderText.text = "Y: " + ySlider.value;
            zSliderText.text = "Z: " + zSlider.value;

        }


        void moveListener(bool a)
        {
            if (mSelectedObject != null)
            {
                xSlider.minValue = -100;
                xSlider.maxValue = 100;

                ySlider.minValue = -100;
                ySlider.maxValue = 100;

                zSlider.minValue = -100;
                zSlider.maxValue = 100;

                xSlider.value = mSelectedObject.transform.parent.localPosition.x;
                ySlider.value = mSelectedObject.transform.parent.localPosition.y;
                zSlider.value = mSelectedObject.transform.parent.localPosition.z;

            }

        }
        private Vector3 ReadObjectXfrom()
        {
            Vector3 p;


            if (mSelectedObject != null)
            {
                p = mSelectedObject.transform.parent.localPosition;
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

            mSelectedObject.transform.parent.localPosition = p;

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
    }
}

