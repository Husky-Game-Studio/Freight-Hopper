using UnityEditor;
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

    [ContextMenu("Update Train")]
    public void UpdateCarts()
    {
        for (int i = 0; i < cartsList.Count; i++)
        {
            DestroyImmediate(cartsList[i].cart);
            DestroyImmediate(cartsList[i].model);
            SetCarts(i, cartsList[i], cartsList[i].cartID);
        }
    }

    void SetCarts(int index, Cart cart, TrainCarts cartID) {
        GameObject model = cartModelsToPickFrom[(int)cartID];
        Vector3 position = GetPosition(index);
        cart.cart = PrefabUtility.InstantiatePrefab(baseCart) as GameObject;
        cart.cart.transform.position = position;
        cart.cart.transform.rotation = this.transform.rotation;
        cart.cart.transform.parent = this.transform;
        cart.cartProperties = cart.cart.GetComponent<CartProperties>();
        cart.cartProperties.setCartIndex(index);
        cart.joint = cart.cart.GetComponent<ConfigurableJoint>();
        if (index == 0)
        {
            cart.joint.connectedBody = locomotive.GetComponent<Rigidbody>();
        }
        else
        {
            cart.joint.connectedBody = cartsList[index - 1].cart.GetComponent<Rigidbody>();
        }
        SoftJointLimit temp = new SoftJointLimit
        {
            limit = gapLength
        };
        cart.joint.linearLimit = temp;
        cart.joint.breakTorque = breakTorque;
        cart.model = PrefabUtility.InstantiatePrefab(model) as GameObject;
        cart.model.transform.position = position;
        cart.model.transform.rotation = this.transform.rotation;
        cart.model.transform.parent = cart.cart.transform;
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
            Cart ourCart = new Cart
            {
                cartID = insertCart
            };
            cartsList.Add(ourCart);
            SetCarts(cartsList.Count - 1, ourCart, insertCart);
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