using UnityEngine;

[CreateAssetMenu()]
public class MeshSettings : UpdatableData
{
    public const int numSupportedLODs = 5;
    public const int numSupportChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96 };

    [SerializeField] private float meshScale = 1;

    [SerializeField, Range(0, numSupportedLODs - 1)] private int chunkSizeIndex;

    // num verts per line of mesh rendered at LOD = 0. Includes the 2 extra verts that are excluded from final mesh but used for calculating normals
    public int NumVertsPerLine => supportedChunkSizes[chunkSizeIndex] + 5;
    public float MeshScale => meshScale;
    public float MeshWorldSize => (NumVertsPerLine - 3) * meshScale;
}