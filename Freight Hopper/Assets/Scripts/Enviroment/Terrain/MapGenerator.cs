using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField, Min(1)] private int mapWidth;
    [SerializeField, Min(1)] private int mapHeight;
    [SerializeField] private float noiseScale;

    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    [SerializeField, Min(1)] private int octaves;
    [SerializeField, Range(0, 1)] private float persistance;
    [SerializeField, Min(1)] private float lacunarity;

    [SerializeField] private Noise.NoiseType noiseType;

    [SerializeField] private bool autoUpdate;
    public bool AutoUpdate => autoUpdate;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, noiseType, offset);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoise(noiseMap);
    }
}