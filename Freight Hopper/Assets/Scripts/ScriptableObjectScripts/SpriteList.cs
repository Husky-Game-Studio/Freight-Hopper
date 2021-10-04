using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Generic/Sprite List"), System.Serializable]
public class SpriteList : ScriptableObject
{
    [SerializeField] private Sprite[] sprites;
    public Sprite[] Sprites => sprites.Clone() as Sprite[];
}