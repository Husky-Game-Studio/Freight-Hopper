using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EchoSlider : MonoBehaviour
{
    [SerializeField] private Slider slider = null;
    [SerializeField] private TextMeshProUGUI echoText;
    [SerializeField] private TextMeshProUGUI labelText;

    private Action<float> callBack;

    private void Start()
    {
        slider.onValueChanged.AddListener(SliderValueChange);
    }

    public void SetListener(Action<float> listener) => callBack = listener;

    public float SliderValue => slider.value;

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