using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

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
        Undo.RegisterCompleteObjectUndo(this, "Generate Terrain");
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
        Undo.RegisterCompleteObjectUndo(this, "Clear Terrain");
        for (int i = 0; i < builtTerrain.Count; i++)
        {
            GameObject go = builtTerrain[i];
            if (go != null)
            {
                Undo.DestroyObjectImmediate(go);
            }
        }

        builtTerrain.Clear();
        this.transform.DestroyImmediateChildren();
    }

#endif
}