using System;
using UnityEngine;

// Note: Enabled functionality is up to the user, this is simply a variable with an additional boolean tied to it
// If this is throwing any random errors, its Unitys fault just remove component and put it back again
// There is nothing that can be done until Unity fixes the bug
// Source: https://gist.github.com/aarthificial/f2dbb58e4dbafd0a93713a380b9612af
[Serializable]
public struct Optional<T>
{
    [SerializeField] private bool enabled;
    public T value;

    public bool Enabled => enabled;

    public Optional(T initialValue)
    {
        enabled = true;
        value = initialValue;
    }

    public void Unenable()
    {
        enabled = false;
    }
}