using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [SerializeField] int3 chunkCount;
    [SerializeField] Material material;

    Dictionary<int3, ChunkMesh> _chunkMeshes = new Dictionary<int3, ChunkMesh>();
    
    [ContextMenu("Generate")]
    public void Generate()
    {
        
    }
}
