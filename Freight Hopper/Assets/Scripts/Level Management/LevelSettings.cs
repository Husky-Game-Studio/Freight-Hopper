using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;

    public Vector3 SpawnPosition => spawnPosition;
}