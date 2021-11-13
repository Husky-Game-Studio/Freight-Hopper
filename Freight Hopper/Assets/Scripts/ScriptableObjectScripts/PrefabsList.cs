using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Generic/Prefabs List"), System.Serializable]
public class PrefabsList : ScriptableObject
{
    [SerializeField] private List<GameObject> prefabs;
    public IList<GameObject> Prefabs => prefabs.AsReadOnly();
}