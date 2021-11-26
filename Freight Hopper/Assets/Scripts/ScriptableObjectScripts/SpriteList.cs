using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Generic/Sprite List"), System.Serializable]
public class SpriteList : ScriptableObject
{
    [SerializeField] private Sprite[] sprites;
    public IList<Sprite> Sprites => Array.AsReadOnly(sprites);
}