using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class TrainBuilder : MonoBehaviour
{
    [SerializeField, Min(1)] private int repeatActionCount = 1;
    [SerializeField, Min(-1),
        Tooltip("Index starts from 0 at the head of the train, -1 gives end of train")]
    private int actionIndex = 1;

    [Header("Joint Settings")]
    [SerializeField] private float locomotiveLength = 42.243f;
    [SerializeField] private float cargoHeight = 5;
    [SerializeField] private float cartLength = 40;
    [SerializeField] private float gapLength = 4;
    [SerializeField] private float jointSnappingLength = 0.0f;
    [SerializeField] private float breakTorque = 3000;

    [Header("Lists and prefabs")]
    [SerializeField] private List<Cart> cartsList;
    [SerializeField] private List<GameObject> cartPrefabs;
    [SerializeField] private List<GameObject> cargoPrefabs;
    [SerializeField] private GameObject baseCart;

    private GameObject locomotive;

    public int ActionIndex
    {
        get
        {
            int index = actionIndex;
            if (actionIndex == -1)
            {
                index = cartsList.Count - 1;
            }
            return index;
        }
    }

    public void IncrementActionIndex()
    {
        actionIndex++;
        if (actionIndex >= cartsList.Count)
        {
            actionIndex = cartsList.Count - 1;
        }
        SceneView.RepaintAll();
    }

    public void DecrementActionIndex()
    {
        actionIndex--;
        if (actionIndex < -1)
        {
            actionIndex = -1;
        }
        SceneView.RepaintAll();
    }

    public bool CanRemoveOrClear => cartsList.Count > 0;
    public bool SelectedCartHasCargo => this.ActionIndex != -1 && cartsList[this.ActionIndex].cargoIDs.Count > 0;

    // This order matters, if this is different from the model order for the list this WON'T work
    public enum TrainCargos
    {
        Cargo1,
        Cargo2,
        None
    }

    // This order matters, if this is different from the model order for the list this WON'T work
    public enum TrainCarts
    {
        EmptyCart1,
        EmptyCart2,
        BillBoardLF,
        BillBoardRF,
        BillBoardB,
        BillBoardF
    }

    [System.Serializable]
    private class Cart
    {
        public CartProperties cartProperties;
        public TrainCarts cartID;
        public List<TrainCargos> cargoIDs = new List<TrainCargos>();
        public GameObject gameObject;
        public ConfigurableJoint joint;
    }

    // Draws a sphere where the action index is
    public void OnDrawGizmosSelected()
    {
        Vector3 position = GetPosition(this.ActionIndex);
        if (actionIndex != -1)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(position, 4);
    }

    public void OnValidate()
    {
        locomotive = this.transform.GetChild(0).gameObject;
        if (actionIndex >= cartsList.Count)
        {
            actionIndex = cartsList.Count - 1;
        }
    }

#if UNITY_EDITOR

    public void UpdateAllTrains()
    {
        TrainBuilder[] trainBuilders = FindObjectsOfType<TrainBuilder>();
        foreach (TrainBuilder builder in trainBuilders)
        {
            builder.UpdateTrain();
        }
    }

    public void UpdateTrain()
    {
        for (int i = 0; i < cartsList.Count; i++)
        {
            SetCart(i, cartsList[i]);
        }
    }

    public void AddCart(TrainCarts cartID, TrainCargos cargoID)
    {
        for (int i = 0; i < repeatActionCount; i++)
        {
            Cart cart = CreateNewCart(cartID, cargoID);
            int index = this.ActionIndex;

            cartsList.Add(cart);

            SetCart(cartsList.Count - 1, cart);
            if (actionIndex != -1)
            {
                for (int j = cartsList.Count - 1; j > index; j--)
                {
                    SwapCarts(cartsList[j], cartsList[j - 1]);
                }
                SetJoint(index + 1);
            }
        }
    }

    public void AddCargo(TrainCargos cargoID)
    {
        if (cargoID == TrainCargos.None)
        {
            return;
        }
        Cart cart = cartsList[this.ActionIndex];
        for (int i = 0; i < repeatActionCount; i++)
        {
            if (cart.cargoIDs.Count > 0 && cart.cargoIDs[cart.cargoIDs.Count - 1] == TrainCargos.None)
            {
                cart.cargoIDs.RemoveAt(cart.cargoIDs.Count - 1);
            }
            cart.cargoIDs.Add(cargoID);
            CreateCargoGameObject(cart, cart.cargoIDs.Count - 1);
        }
    }

    public void RemoveCargo()
    {
        Cart cart = cartsList[this.ActionIndex];
        for (int i = 0; i < repeatActionCount; i++)
        {
            if (cart.cargoIDs.Count <= 0)
            {
                break;
            }
            cart.cargoIDs.RemoveAt(cart.cargoIDs.Count - 1);

            SetCart(this.ActionIndex, cart);
            if (this.ActionIndex + 1 < cartsList.Count)
            {
                SetJoint(this.ActionIndex + 1);
            }
        }
    }

    public void ReplaceCart(TrainCarts cartID, TrainCargos cargoID)
    {
        Cart cart = cartsList[this.ActionIndex];
        cart.cartID = cartID;
        cart.cargoIDs.Clear();
        cart.cargoIDs.Add(cargoID);
        SetCart(this.ActionIndex, cart);
        if (this.ActionIndex + 1 < cartsList.Count)
        {
            SetJoint(this.ActionIndex + 1);
        }
    }

    public void RemoveCart()
    {
        for (int i = 0; i < repeatActionCount; i++)
        {
            if (cartsList.Count <= 0)
            {
                break;
            }

            if (actionIndex != -1)
            {
                if (actionIndex >= cartsList.Count)
                {
                    break;
                }
                for (int j = this.actionIndex; j < cartsList.Count - 1; j++)
                {
                    SwapCarts(cartsList[j], cartsList[j + 1]);
                }
                SetJoint(this.ActionIndex);
            }

            DestroyImmediate(cartsList[cartsList.Count - 1].gameObject);
            cartsList.RemoveAt(cartsList.Count - 1);
        }
    }

    public void Clear()
    {
        if (cartsList.Count <= 0)
        {
            return;
        }

        foreach (var cart in cartsList)
        {
            DestroyImmediate(cart.gameObject);
        }
        cartsList.Clear();
    }

    private void SetCart(int index, Cart cart)
    {
        Vector3 position = GetPosition(index);
        if (cart.gameObject != null)
        {
            DestroyImmediate(cart.gameObject);
        }
        cart.gameObject = PrefabUtility.InstantiatePrefab(baseCart) as GameObject;
        cart.gameObject.transform.position = position;
        cart.gameObject.transform.rotation = this.transform.rotation;
        cart.gameObject.transform.parent = this.transform;
        cart.gameObject.name = cart.gameObject.name + " " + index;
        cart.cartProperties = cart.gameObject.GetComponent<CartProperties>();
        cart.cartProperties.SetCartIndex(index);
        cart.joint = cart.gameObject.GetComponent<ConfigurableJoint>();
        SetJoint(index);
        SoftJointLimit temp = new SoftJointLimit
        {
            limit = gapLength
        };
        cart.joint.linearLimit = temp;
        cart.joint.breakTorque = breakTorque;

        CreateCartGameObject(cart, position);
        for (int i = 0; i < cart.cargoIDs.Count; i++)
        {
            CreateCargoGameObject(cart, i);
        }
    }

    private void SetJoint(int index)
    {
        Cart cart = cartsList[index];
        if (index == 0)
        {
            cart.joint.connectedBody = locomotive.GetComponent<Rigidbody>();
        }
        else
        {
            cart.joint.connectedBody = cartsList[index - 1].gameObject.GetComponent<Rigidbody>();
        }
    }

    private Vector3 GetPosition(int index)
    {
        return this.transform.position + (this.transform.forward * -((index * cartLength / 2) + (locomotiveLength / 2) + ((index + 1) * (gapLength + jointSnappingLength))));
    }

    private void CreateCartGameObject(Cart cart, Vector3 position)
    {
        GameObject cartModelPrefab = cartPrefabs[(int)cart.cartID];
        GameObject cartModel = PrefabUtility.InstantiatePrefab(cartModelPrefab) as GameObject;
        cartModel.transform.position = position;
        cartModel.transform.rotation = this.transform.rotation;
        cartModel.transform.parent = cart.gameObject.transform;
    }

    private void CreateCargoGameObject(Cart cart, int index)
    {
        if (cart.cargoIDs.Count > 0 && cart.cargoIDs[index] != TrainCargos.None)
        {
            GameObject cargoModelPrefab = cargoPrefabs[(int)cart.cargoIDs[index]];
            GameObject go = PrefabUtility.InstantiatePrefab(cargoModelPrefab) as GameObject;

            go.transform.rotation = this.transform.rotation;
            go.transform.parent = cart.gameObject.transform;
            go.transform.localPosition = Vector3.zero + (Vector3.up * index * cargoHeight);
            go.name = go.name + " " + index;
        }
    }

    private Cart CreateNewCart(TrainCarts cartID, TrainCargos cargoID)
    {
        Cart cart = new Cart
        {
            cartID = cartID,
        };
        cart.cargoIDs.Add(cargoID);

        return cart;
    }

    private void SwapCarts(Cart a, Cart b)
    {
        int indexA = a.cartProperties.IndexOfCart;
        int indexB = b.cartProperties.IndexOfCart;
        cartsList[indexA] = b;
        cartsList[indexB] = a;
        a.cartProperties.SetCartIndex(indexB);
        b.cartProperties.SetCartIndex(indexA);
        SetJoint(indexB);

        Vector3 aPosition = a.gameObject.transform.position;
        Vector3 bPosition = b.gameObject.transform.position;
        a.gameObject.transform.position = bPosition;
        b.gameObject.transform.position = aPosition;

        Quaternion aRotation = a.gameObject.transform.rotation;
        Quaternion bRotation = b.gameObject.transform.rotation;
        a.gameObject.transform.rotation = bRotation;
        b.gameObject.transform.rotation = aRotation;
    }

#endif
}