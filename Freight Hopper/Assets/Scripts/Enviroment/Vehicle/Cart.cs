using UnityEngine;
using System;

public class Cart
{
    public Rigidbody rb;
    public CartProperties properties;
    public HoverController hoverController;
    public Destructable destructable;
    public PID uprightPID;

    public event Action<int> DestoryCart;

    public Cart(Rigidbody rb)
    {
        this.rb = rb;
        this.properties = rb.GetComponent<CartProperties>();
        this.hoverController = rb.GetComponentInChildren<HoverController>();
        this.destructable = rb.GetComponent<Destructable>();
        this.uprightPID = new PID();
        uprightPID.Initialize(new PID.Data(3f, 0.1f, 0.2f)); // This needs to be done somewhere where it can be tweaked by game designers
    }

    public void DestroyCartFunc()
    {
        int cartIndex = properties.IndexOfCart;
        DestoryCart?.Invoke(cartIndex);
    }
}