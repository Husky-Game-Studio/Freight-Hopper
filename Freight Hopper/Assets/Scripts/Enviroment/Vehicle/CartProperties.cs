using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartProperties : MonoBehaviour
{
    [SerializeField] private int indexOfCart;
    public int IndexOfCart => indexOfCart;

    public void SetCartIndex(int index)
    {
        indexOfCart = index;
    }
}