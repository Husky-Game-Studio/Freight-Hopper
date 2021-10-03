using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BillBoardChanger : MonoBehaviour
{
    [SerializeField] private Material[] billboards;
    private MeshRenderer ourRenderer;

    private void Awake()
    {
        ourRenderer = GetComponent<MeshRenderer>();
        SwapBillboard();
    }

    public void SwapBillboard()
    {
        ourRenderer.material = billboards[Random.Range(0, billboards.Length)];
    }
}