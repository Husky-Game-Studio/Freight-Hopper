using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EditorObject", fileName = "EditorObject")]
public class LevelEditorObjects : ScriptableObject
{
    public Sprite buttonPhoto;
    public string objectID;
    public GameObject objectPrefab; 

}
