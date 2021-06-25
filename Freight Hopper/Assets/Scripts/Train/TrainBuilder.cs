using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainBuilder : MonoBehaviour
{
    [SerializeField] private List<Cart> cartsList;
    [SerializeField] private TrainCarts insertCart;
    [SerializeField] private float locomotiveLength = 42.243f;
    [SerializeField] private float cartLength = 40;
    [SerializeField] private float gapLength = 4;
    [SerializeField] private float jointSnappingLength = 0.0f;
    [SerializeField] private float breakTorque = 3000;
    [SerializeField] private TrainCargos insertedCargo;
    [SerializeField] private List<GameObject> cartModelsToPickFrom;
    [SerializeField] private GameObject baseCart;
    
    private GameObject locomotive;

    // This order matters, if this is different from the model order for the list this WON'T work
    public enum TrainCargos
    {
        None,
        Cargo1,
        Cargo2,    
    }
    
    
    // This order matters, if this is different from the model order for the list this WON'T work
    public enum TrainCarts
    {
        EmptyCart1,
        EmptyCart2,
        BillBoardCarrier,
        BillBoardLFModel,
        BillBoardRFModel
    }
    [System.Serializable]
    struct Cart
    {
        public GameObject cart;
        public ConfigurableJoint joint;
        public GameObject model;
    }
    
    void OnValidate(){
        locomotive = this.transform.GetChild(0).gameObject;
    }
    
    // Adds cart to end of chain
    [ContextMenu("Add Cart")]
    public void AddCart()
    {

            GameObject model = cartModelsToPickFrom[(int)insertCart];

            Cart ourCart = new Cart();
            Vector3 position = Vector3.forward * -(((cartsList.Count * cartLength/2) + locomotiveLength/2) + (cartsList.Count + 1) * (gapLength + jointSnappingLength));
            ourCart.cart = Instantiate(baseCart, position, Quaternion.identity, this.transform);
            ourCart.joint = ourCart.cart.GetComponent<ConfigurableJoint>();
            if(cartsList.Count == 0){
                ourCart.joint.connectedBody = locomotive.GetComponent<Rigidbody>();
            }
            else
            {
                ourCart.joint.connectedBody = cartsList[cartsList.Count-1].cart.GetComponent<Rigidbody>();
            }
  
            SoftJointLimit temp = new SoftJointLimit();
            temp.limit = gapLength;
            ourCart.joint.linearLimit = temp;
            ourCart.joint.breakTorque = breakTorque;
            ourCart.model = Instantiate(model, position, Quaternion.identity, ourCart.cart.transform);
            
            cartsList.Add(ourCart);
        
        
    }

    private void AddCargo()
    {
        
        
    }

    // Removes last cart in the chain
    [ContextMenu("Remove Cart")] 
    public void RemoveCart()
    {
        if(cartsList.Count == 0)
        {
            return;
        }
   
        DestroyImmediate(cartsList[cartsList.Count - 1].cart);
        cartsList.RemoveAt(cartsList.Count - 1);
    }
    [ContextMenu("Clear")]
    public void Clear(){
        if(cartsList.Count == 0)
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
