using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[ExecuteInEditMode]
public class PrefabSwapper : MonoBehaviour
{
    public Transform sourceGameobject;
    public List<GameObject> prefabs;
    public Memory<int> currentIndex;

    private void Update()
    {
        if (currentIndex.current == currentIndex.old)
        {
            return;
        }
        if (prefabs.Count < 1 || currentIndex.current >= prefabs.Count || currentIndex.current < 0 || prefabs[currentIndex.current] == null)
        {
            return;
        }
        GameObject go = PrefabUtility.InstantiatePrefab(prefabs[currentIndex.current], sourceGameobject.parent) as GameObject;
        DestroyImmediate(sourceGameobject.gameObject);
        sourceGameobject = go.transform;
        currentIndex.UpdateOld();
    }
}

#endif