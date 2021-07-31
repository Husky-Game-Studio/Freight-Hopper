using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Level Editor Objects")]
public class LevelEditorObjects : ScriptableObject
{
    [SerializeField] private Sprite buttonPhoto;
    [SerializeField] private string objectID;
    [SerializeField] private GameObject objectPrefab; 

}
