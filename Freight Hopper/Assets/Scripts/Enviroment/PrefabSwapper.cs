using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class PrefabSwapper : MonoBehaviour
{
    public Transform sourceGameobject;
    public List<GameObject> prefabs;
    public int currentIndex;


    [ContextMenu("Switch")]
    private void Switch()
    {

        if (prefabs.Count < 1 || currentIndex >= prefabs.Count || currentIndex < 0 || prefabs[currentIndex] == null)
        {
            return;
        }
        CreateSelected();
    }
    private void CreateSelected()
    {
        GameObject go = PrefabUtility.InstantiatePrefab(prefabs[currentIndex], this.transform.GetChild(0)) as GameObject;
        if(sourceGameobject != null)
        {
            DestroyImmediate(sourceGameobject.gameObject);
        }
        sourceGameobject = go.transform;
    }
}

#endif