using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainData : MonoBehaviour
{
    [SerializeField] private float uniformScale;
    [SerializeField] private bool useFlatShading;
    [SerializeField] private bool useFalloff;

    [SerializeField] private float meshHeightMultiplier;
    [SerializeField] private AnimationCurve meshHeightCurve;

    public float minHeight => uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
    public float maxHeight => uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
}