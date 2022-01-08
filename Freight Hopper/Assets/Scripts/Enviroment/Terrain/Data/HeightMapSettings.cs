using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{
    public NoiseSettings noiseSettings;

    [SerializeField, Min(0.1f)] private float heightMultiplier;
    [SerializeField] private AnimationCurve heightCurve;
    public float MinHeight => heightMultiplier * heightCurve.Evaluate(0);
    public float MaxHeight => heightMultiplier * heightCurve.Evaluate(1);

    public float HeightMultiplier => heightMultiplier;
    public AnimationCurve HeightCurve => heightCurve;
}