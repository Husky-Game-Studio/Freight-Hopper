using UnityEngine;

[System.Serializable]
public class HoverPresets
{
    [SerializeField] private Presets currentPreset = Presets.Default;

    private enum Presets
    {
        Default,
        Stiff,
        Spongy,
    }

    [SerializeField, ReadOnly] private PID.Data currentData;

    private PID.Data[] presetData =
    {
        new PID.Data(10.5f, 0.2f, 1.5f),
        new PID.Data(3.5f, 0.2f, 1.5f),
        new PID.Data(3.5f, 0, 0.1f)
    };

    public PID.Data CurrentPreset()
    {
        currentData = presetData[(int)currentPreset];
        return currentData;
    }
}