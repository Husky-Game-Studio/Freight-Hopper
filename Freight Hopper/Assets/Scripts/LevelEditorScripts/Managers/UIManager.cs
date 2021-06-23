using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    public Camera MainCamera = null;
    public GameObject mSelectedObject = null;
    public float holdXvalue;

    //Texts 
    public Text xSliderText = null;
    public Text ySlidertext = null;
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

    }

    // Update is called once per frame
    void Update()
    {
            moveShape();
  
    }

    void moveShape()
    {
        mSelectedObject.transform.localPosition = new Vector3(xSlider.value, ySlider.value, zSlider.value);

        xSliderText.text = "X: " + xSlider.value;
        ySlidertext.text = "Y: " + ySlider.value;
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

            xSlider.value = mSelectedObject.transform.localPosition.x;
            ySlider.value = mSelectedObject.transform.localPosition.y;
            zSlider.value = mSelectedObject.transform.localPosition.z;

        }

    }
}

