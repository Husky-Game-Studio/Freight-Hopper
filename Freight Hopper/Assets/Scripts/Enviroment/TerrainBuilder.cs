using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TerrainBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private GameObject terrain;
    [SerializeField] private List<GameObject> builtTerrain = new List<GameObject>();
    [SerializeField] private string terrainName = "Terrain";
    [SerializeField] private FloatConst terrainWidth;
    [SerializeField] private Vector2Int size;

    [ContextMenu("Generate")]
    public void Generate()
    {
        Clear();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(terrain, this.transform) as GameObject;
                Undo.RegisterCreatedObjectUndo(go, "Generate Terrain");
                go.name = terrainName + " " + x + ", " + y;
                go.transform.position = this.transform.position + (this.transform.right * x * terrainWidth.Value) + (this.transform.forward * y * terrainWidth.Value);
                builtTerrain.Add(go);
            }
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        foreach (GameObject go in builtTerrain)
        {
            if (go != null)
            {
                Undo.DestroyObjectImmediate(go);
            }
        }

        builtTerrain.Clear();
    }

#endif
}