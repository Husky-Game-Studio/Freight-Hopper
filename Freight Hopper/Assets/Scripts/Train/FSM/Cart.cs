using UnityEngine;
using System;

public class Cart
{
    public PhysicsManager physicsManager;
    public Rigidbody rb;
    public CartProperties properties;
    public Destructable destructable;

    public event Action<int> DestoryCart;

    public Cart(PhysicsManager physicsManager)
    {
        this.physicsManager = physicsManager;
        this.rb = physicsManager.rb;
        this.properties = rb.GetComponent<CartProperties>();
        this.destructable = rb.GetComponent<Destructable>();
    }

    public void DestroyCartFunc()
    {
        int cartIndex = properties.IndexOfCart;
        DestoryCart?.Invoke(cartIndex);
    }
}