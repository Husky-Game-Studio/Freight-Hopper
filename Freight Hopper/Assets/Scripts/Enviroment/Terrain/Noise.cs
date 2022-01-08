using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public static class Noise
{
    public enum NormalizeMode
    { Local, Global };

    public enum NoiseType
    {
        cnoise,
        snoise,
        srnoise
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter)
    {
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        System.Random prng = new System.Random(settings.Seed);
        Vector2[] octaveOffsets = new Vector2[settings.Octaves];
        for (int i = 0; i < settings.Octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.Offset.x + sampleCenter.x;
            float offsetY = prng.Next(-100000, 100000) - settings.Offset.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.Persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        float[,] noiseMap = new float[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < settings.Octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.Scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.Scale * frequency;

                    float noiseValue;
                    if (settings.Type == NoiseType.cnoise)
                    {
                        noiseValue = (noise.cnoise(new float2(sampleX, sampleY)) * 2) - 1;
                    }
                    else if (settings.Type == NoiseType.snoise)
                    {
                        noiseValue = (noise.snoise(new float2(sampleX, sampleY)) * 2) - 1;
                    }
                    else
                    {
                        noiseValue = (noise.srnoise(new float2(sampleX, sampleY)) * 2) - 1;
                    }
                    noiseHeight += noiseValue * amplitude;

                    amplitude *= settings.Persistance;
                    frequency *= settings.Lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        maxLocalNoiseHeight = 1 / (1 - Mathf.Pow(settings.Persistance, 1.5f) / 2);
        minLocalNoiseHeight = -maxLocalNoiseHeight;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (settings.NormalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / maxPossibleHeight;
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    [SerializeField, Min(0.01f)] private float scale = 50;

    [SerializeField, Min(1)] private int octaves = 6;
    [SerializeField, Range(0, 1)] private float persistance = 0.6f;
    [SerializeField, Min(1)] private float lacunarity = 2;

    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    [SerializeField] private Noise.NoiseType type;
    [SerializeField] private Noise.NormalizeMode normalizeMode;

    public float Scale => scale;
    public int Octaves => octaves;
    public float Persistance => persistance;
    public float Lacunarity => lacunarity;
    public int Seed => seed;
    public Vector2 Offset => offset;
    public Noise.NoiseType Type => type;
    public Noise.NormalizeMode NormalizeMode => normalizeMode;
}