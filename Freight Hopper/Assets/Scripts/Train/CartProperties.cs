using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartProperties : MonoBehaviour
{
    [SerializeField] private int indexOfCart;
    public int IndexOfCart => indexOfCart;

    public void setCartIndex(int index)
    {
        indexOfCart = index;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
