using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainBuilder : MonoBehaviour
{
    [SerializeField] private List<Cart> cartsList;
    [SerializeField] private TrainCarts insertCart;
    [SerializeField, Min(1)] private int repeatActionCount = 1;

    [SerializeField] private float locomotiveLength = 42.243f;
    [SerializeField] private float cartLength = 40;
    [SerializeField] private float gapLength = 4;
    [SerializeField] private float jointSnappingLength = 0.0f;
    [SerializeField] private float breakTorque = 3000;

    //[SerializeField] private TrainCargos insertedCargo;
    [SerializeField] private List<GameObject> cartModelsToPickFrom;
    //[SerializeField] private List<GameObject> cargoModelsToPickFrom;

    [SerializeField] private GameObject baseCart;

    private GameObject locomotive;

    // This order matters, if this is different from the model order for the list this WON'T work
    /*public enum TrainCargos
    {
        None,
        Cargo1,
        Cargo2,
    }*/

    // This order matters, if this is different from the model order for the list this WON'T work
    public enum TrainCarts
    {
        EmptyCart1,
        EmptyCart2,
        BillBoardCarrier,
        BillBoardLFModel,
        BillBoardRFModel,
        CargoCart11,
        CargoCart12,
        CargoCart21,
        CargoCart22
    }

    [System.Serializable]
    private class Cart
    {
        public CartProperties cartProperties;
        public TrainCarts cartID;
        public GameObject cart;
        public ConfigurableJoint joint;
        public GameObject model;
    }

    private void OnValidate()
    {
        locomotive = this.transform.GetChild(0).gameObject;
    }

    [ContextMenu("Update All Trains")]
    public void UpdateAllTrains()
    {
        TrainBuilder[] trainBuilders = FindObjectsOfType<TrainBuilder>();
        foreach (TrainBuilder builder in trainBuilders)
        {
            builder.UpdateCarts();
        }
    }

    [ContextMenu("Update Cart")]
    public void UpdateCarts()
    {
        for (int i = 0; i < cartsList.Count; i++)
        {
            DestroyImmediate(cartsList[i].cart);
            cartsList[i].cart = Instantiate(baseCart, GetPosition(i), this.transform.rotation, this.transform);
            cartsList[i].joint = cartsList[i].cart.GetComponent<ConfigurableJoint>();
            if (i == 0)
            {
                cartsList[i].joint.connectedBody = locomotive.GetComponent<Rigidbody>();
            }
            else
            {
                cartsList[i].joint.connectedBody = cartsList[i - 1].cart.GetComponent<Rigidbody>();
            }

            SoftJointLimit temp = new SoftJointLimit
            {
                limit = gapLength
            };
            cartsList[i].joint.linearLimit = temp;
            cartsList[i].joint.breakTorque = breakTorque;

            DestroyImmediate(cartsList[i].model);
            GameObject model = cartModelsToPickFrom[(int)cartsList[i].cartID];
            cartsList[i].model = Instantiate(model, GetPosition(i), this.transform.rotation, cartsList[i].cart.transform);
        }
    }

    public Vector3 GetPosition(int index)
    {
        return this.transform.position + (this.transform.forward * -((index * cartLength / 2) + (locomotiveLength / 2) + ((index + 1) * (gapLength + jointSnappingLength))));
    }

    // Adds cart to end of chain
    [ContextMenu("Add Cart")]
    public void AddCart()
    {
        for (int i = 0; i < repeatActionCount; i++)
        {
            GameObject model = cartModelsToPickFrom[(int)insertCart];

            Cart ourCart = new Cart
            {
                cartID = insertCart
            };
            Vector3 position = GetPosition(cartsList.Count);
            ourCart.cart = Instantiate(baseCart, position, this.transform.rotation, this.transform);
            ourCart.cartProperties = ourCart.cart.GetComponent<CartProperties>();
            ourCart.cartProperties.setCartIndex(cartsList.Count + 1);
            ourCart.joint = ourCart.cart.GetComponent<ConfigurableJoint>();
            if (cartsList.Count == 0)
            {
                ourCart.joint.connectedBody = locomotive.GetComponent<Rigidbody>();
            }
            else
            {
                ourCart.joint.connectedBody = cartsList[cartsList.Count - 1].cart.GetComponent<Rigidbody>();
            }

            SoftJointLimit temp = new SoftJointLimit
            {
                limit = gapLength
            };
            ourCart.joint.linearLimit = temp;
            ourCart.joint.breakTorque = breakTorque;
            ourCart.model = Instantiate(model, position, this.transform.rotation, ourCart.cart.transform);

            cartsList.Add(ourCart);
        }
    }

    // Removes last cart in the chain
    [ContextMenu("Remove Cart")]
    public void RemoveCart()
    {
        for (int i = 0; i < repeatActionCount; i++)
        {
            if (cartsList.Count == 0)
            {
                return;
            }

            DestroyImmediate(cartsList[cartsList.Count - 1].cart);
            cartsList.RemoveAt(cartsList.Count - 1);
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (cartsList.Count == 0)
        {
            return;
        }

        foreach (var cart in cartsList)
        {
            DestroyImmediate(cart.cart);
        }
        cartsList.Clear();
    }
}