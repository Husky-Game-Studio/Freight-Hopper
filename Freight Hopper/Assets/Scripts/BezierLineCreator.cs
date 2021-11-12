using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierLineCreator : PathCreation.Examples.PathSceneTool
{
    public FloatConst thickness;
    [SerializeField] private GameObject meshHolder;
    [SerializeField] private Material material;
    [SerializeField] private LineRenderer lineRenderer;

    protected override void PathUpdated()
    {
        if (pathCreator != null)
        {
            AssignMeshComponents();
            AssignMaterials();
            CreateMesh();
        }
    }

    private void CreateMesh()
    {
        var pathInstruction = PathCreation.EndOfPathInstruction.Stop;

        lineRenderer.positionCount = this.path.LastVertexIndex + 1;
        for (int i = 0; i <= this.path.LastVertexIndex; i++)
        {
            lineRenderer.SetPosition(i, this.path.GetPoint(i));
        }
        lineRenderer.widthMultiplier = thickness.Value;
        lineRenderer.Simplify(0.1f);

        lineRenderer.loop = path.isClosedLoop;
    }

    private void AssignMeshComponents()
    {
        if (meshHolder == null)
        {
            if (this.transform.GetChild(0).name == "Mesh Holder")
            {
                meshHolder = this.transform.GetChild(0).gameObject;
            }
            else
            {
                meshHolder = new GameObject("Mesh Holder");
            }
        }

        meshHolder.transform.rotation = Quaternion.identity;
        meshHolder.transform.position = Vector3.zero;
        meshHolder.transform.localScale = Vector3.one;

        // Ensure mesh renderer and filter components are assigned
        if (!meshHolder.gameObject.GetComponent<LineRenderer>())
        {
            meshHolder.gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer = meshHolder.GetComponent<LineRenderer>();
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        if (meshHolder.GetComponent<MeshRenderer>() != null)
        {
            DestroyImmediate(meshHolder.GetComponent<MeshRenderer>());
        }
        if (meshHolder.GetComponent<MeshFilter>() != null)
        {
            DestroyImmediate(meshHolder.GetComponent<MeshFilter>());
        }
        if (this.GetComponent<PathCreation.Examples.CylinderMeshCreator>() != null)
        {
            DestroyImmediate(GetComponent<PathCreation.Examples.CylinderMeshCreator>());
        }
    }

    private void AssignMaterials()
    {
        if (lineRenderer != null)
        {
            this.lineRenderer.sharedMaterial = material;
        }
    }
}