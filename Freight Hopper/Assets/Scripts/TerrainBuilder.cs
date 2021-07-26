using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TerrainBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private GameObject terrain;
    private List<GameObject> builtTerrain = new List<GameObject>();
    [SerializeField] private string terrainName = "Terrain";
    [SerializeField] private FloatConst terrainWidth;
    [SerializeField] private Vector2 size;

    [ContextMenu("Generate")]
    public void Generate()
    {
        for (int i = 0; i < builtTerrain.Count; i++)
        {
            DestroyImmediate(builtTerrain[i]);
        }
        builtTerrain.Clear();
        Vector3 position = this.transform.position;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(terrain, this.transform) as GameObject;
                go.name = terrainName + " " + x + ", " + y;
                go.transform.position = position + (this.transform.right * x * terrainWidth.Value) + (this.transform.forward * y * terrainWidth.Value);
                builtTerrain.Add(go);
            }
        }
    }

#endif
}