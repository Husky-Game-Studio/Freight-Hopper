using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EchoSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI echoText;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] float stepSize = 0.1f;
    private Action<float> callBack;
    
    private void Start()
    {
        slider.onValueChanged.AddListener(SliderValueChange);
    }

    public void SetListener(Action<float> listener) => callBack = listener;

    public void SetLabel(string label) => labelText.text = label;

    public void SetSliderValue(float value)
    {
        slider.value = value;
        SliderValueChange(value);
    }

    public void InitSliderRange(float min, float max, float value)
    {
        slider.minValue = min;
        slider.maxValue = max;
        SetSliderValue(value);
    }

    private void SliderValueChange(float value)
    {
        if (stepSize > 0 && !slider.wholeNumbers)
        {
            float steppedValue = Mathf.Round(value / stepSize) * stepSize;
            value = steppedValue;
            slider.value = value;
        }
        if (slider.wholeNumbers)
        {
            echoText.text = value.ToString("0");
        }
        else
        {
            echoText.text = value.ToString("0.00");
        }
        
        if (callBack != null)
        {
            callBack(value);
        }
    }
}