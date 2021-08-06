using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

[CreateAssetMenu(menuName = "Scriptable Objects/Road Slice")]
public class RoadSlice : ScriptableObject
{
    [SerializeField] private Optional<float> railSize;
    [SerializeField] private Optional<float> railSeperationDistance;

    [SerializeField] private Vector3[] points;
    [SerializeField] private Vector2Int[] connections;

    [SerializeField]
    private Vector2[] uvs;

    public Optional<float> RailSize => railSize;
    public Optional<float> RailSeperationDistance => railSeperationDistance;

    public Vector3[] Points() => points.Clone() as Vector3[];

    public Vector2Int[] Connections() => connections.Clone() as Vector2Int[];

    public Vector2[] Uvs() => uvs.Clone() as Vector2[];
}