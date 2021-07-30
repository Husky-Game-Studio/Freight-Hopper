using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HGSLevelEditor;

public class ButtonLoad : MonoBehaviour
{
    [SerializeField] private GameObject spawnObject;

    GhostObjectMaker ghost;
    void Start()
    {
        ghost = GhostObjectMaker.GetInstance();
    }

    public void SpawnObject() {

        ghost.SpawnGhost(spawnObject);
    }
}
