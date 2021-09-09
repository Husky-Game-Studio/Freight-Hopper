using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class TrainBuilder : MonoBehaviour
{
    [SerializeField] private Optional<PathCreation.PathCreator> linkedPath;
    public bool LinkedPathSet => linkedPath.Enabled && linkedPath.value.path != null;
    [SerializeField, Min(1)] private int repeatActionCount = 1;
    [SerializeField, Min(-1),
        Tooltip("Index starts from 0 at the head of the train, -1 gives end of train")]
    private int actionIndex = 1;

    [Header("Joint Settings")]
    [SerializeField] private FloatConst locomotiveLength;
    [SerializeField] private FloatConst cargoHeight;
    [SerializeField] private FloatConst cartLength;
    [SerializeField] private FloatConst gapLength;
    [SerializeField] private FloatConst jointSnappingLength;
    [SerializeField] private float breakTorque = 3000;

    [Header("Lists and prefabs")]
    [SerializeField] private List<Cart> cartsList = new List<Cart>();
    [SerializeField] private List<GameObject> cartPrefabs;
    [SerializeField] private List<GameObject> cargoPrefabs;
    [SerializeField] private GameObject baseCart;

    private GameObject locomotive;
    private int cartIndexSelection = 0;
    private int cargoIndexSelection = 0;
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

    // This order matters, if this is different from the model order for the list this WON'T work
    public enum TrainCargos
    {
        None,
        BoxCargo,
        PillCargo,
    }

    // This order matters, if this is different from the model order for the list this WON'T work
    public enum TrainCarts
    {
        EmptyCart,
        EmptyCartWalls,
        EmptyCartLasers,
        BillBoardLF,
        BillBoardRF,
        BillBoardB,
        BillBoardF,
        TurretCart
    }

    [System.Serializable]
    private class Cart
    {
        public CartProperties cartProperties;
        public int cartID;
        public List<int> cargoIDs = new List<int>();
        public GameObject gameObject;
        public ConfigurableJoint joint;
    }

#if UNITY_EDITOR

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
        if (linkedPath.Enabled && linkedPath.value.path != null)
        {
            //pathDirectionHelper.pathObject = linkedPath.value.path.gameObject;
            //float t = linkedPath.value.path.FindClosestT(locomotive.transform.position);

            float t = linkedPath.value.path.GetClosestTimeOnPath(locomotive.transform.position);
            Vector3 position = linkedPath.value.path.GetPointAtTime(t);
            locomotive.transform.position = position;
            float offsetDistance = linkedPath.value.GetComponent<TrainRailLinker>().Height;
            locomotive.transform.position += linkedPath.value.path.GetNormal(t) * offsetDistance;
            locomotive.transform.rotation = linkedPath.value.path.GetRotation(t);
        }
        EditorUtility.SetDirty(this);
    }

    public void SaveModelSettings(int cartID, int cargoID)
    {
        cartIndexSelection = cartID;
        cargoIndexSelection = cargoID;
    }

    public (int, int) LoadModelSettings()
    {
        return (cartIndexSelection, cargoIndexSelection);
    }

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
        OnValidate();
        List<GameObject> deleteExceptions = new List<GameObject>
        {
            locomotive
        };
        for (int i = 0; i < cartsList.Count; i++)
        {
            if (cartsList[i].gameObject != null)
            {
                deleteExceptions.Add(cartsList[i].gameObject);
            }
        }
        this.transform.DestroyImmediateChildren(deleteExceptions);
        for (int i = 0; i < cartsList.Count; i++)
        {
            UpdateCart(i, cartsList[i]);
        }
    }

    public void AddCart(int cartID, int cargoID)
    {
        Undo.RegisterCompleteObjectUndo(this, "Add Cart");
        for (int i = 0; i < repeatActionCount; i++)
        {
            Cart cart = CreateNewCart(cartID, cargoID);
            int index = this.ActionIndex;

            cartsList.Add(cart);

            UpdateCart(cartsList.Count - 1, cart);
            if (actionIndex != -1)
            {
                for (int j = cartsList.Count - 1; j > index; j--)
                {
                    SwapCarts(cartsList[j], cartsList[j - 1]);
                }
                UpdateJoint(index + 1);
            }
        }
        UpdateTrain();
    }

    public void AddCargo(int cargoID)
    {
        Undo.RegisterCompleteObjectUndo(this, "Add Cargo");
        if ((TrainCargos)cargoID == TrainCargos.None)
        {
            return;
        }
        Cart cart = cartsList[this.ActionIndex];
        for (int i = 0; i < repeatActionCount; i++)
        {
            if (cart.cargoIDs.Count > 0 && (TrainCargos)cart.cargoIDs[cart.cargoIDs.Count - 1] == TrainCargos.None)
            {
                cart.cargoIDs.RemoveAt(cart.cargoIDs.Count - 1);
            }
            cart.cargoIDs.Add(cargoID);
            CreateCargoGameObject(cart, cart.cargoIDs.Count - 1);
        }
        UpdateTrain();
    }

    public void RemoveCargo()
    {
        Undo.RegisterCompleteObjectUndo(this, "Remove Cargo");
        Cart cart = cartsList[this.ActionIndex];
        for (int i = 0; i < repeatActionCount; i++)
        {
            if (cart.cargoIDs.Count <= 0)
            {
                break;
            }
            cart.cargoIDs.RemoveAt(cart.cargoIDs.Count - 1);

            UpdateCart(this.ActionIndex, cart);
            if (this.ActionIndex + 1 < cartsList.Count)
            {
                UpdateJoint(this.ActionIndex + 1);
            }
        }
        UpdateTrain();
    }

    public void ReplaceCart(int cartID, int cargoID)
    {
        Undo.RegisterCompleteObjectUndo(this, "Replace Cart");
        Cart cart = cartsList[this.ActionIndex];
        cart.cartID = cartID;
        cart.cargoIDs.Clear();
        cart.cargoIDs.Add(cargoID);
        UpdateCart(this.ActionIndex, cart);
        if (this.ActionIndex + 1 < cartsList.Count)
        {
            UpdateJoint(this.ActionIndex + 1);
        }
        UpdateTrain();
    }

    public void RemoveCart()
    {
        if (cartsList.Count <= 0)
        {
            return;
        }
        Undo.RegisterCompleteObjectUndo(this, "Remove Cart");
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
                UpdateJoint(this.ActionIndex);
            }

            Undo.DestroyObjectImmediate(cartsList[cartsList.Count - 1].gameObject);
            cartsList.RemoveAt(cartsList.Count - 1);
        }
        UpdateTrain();
    }

    public void Clear()
    {
        if (cartsList.Count <= 0)
        {
            return;
        }
        Undo.RegisterCompleteObjectUndo(this, "Clear Train");

        for (int i = 0; i < cartsList.Count; i++)
        {
            GameObject go = cartsList[i].gameObject;
            if (go != null)
            {
                Undo.DestroyObjectImmediate(cartsList[i].gameObject);
            }
        }
        cartsList.Clear();
        ClearCartGameObjects();
    }

    private void ClearCartGameObjects()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = this.transform.GetChild(i).gameObject;
            CartProperties cartProperties = child.GetComponent<CartProperties>();
            if (cartProperties != null && child.gameObject != locomotive)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private void UpdateCart(int index, Cart cart)
    {
        Vector3 position = GetPosition(index);
        Quaternion rotation = GetRotation(index);
        if (cart.gameObject != null)
        {
            Undo.DestroyObjectImmediate(cart.gameObject);
        }

        cart.gameObject = PrefabUtility.InstantiatePrefab(baseCart, this.transform) as GameObject;
        cart.gameObject.transform.position = position;
        cart.gameObject.transform.rotation = rotation;
        cart.gameObject.name = cart.gameObject.name + " " + index;
        Undo.RegisterCreatedObjectUndo(cart.gameObject, "Created cart model");

        cart.cartProperties = cart.gameObject.GetComponent<CartProperties>();
        cart.cartProperties.SetCartIndex(index);

        SetJoint(index, cart);

        CreateCartGameObjectModel(cart, position);
        for (int i = 0; i < cart.cargoIDs.Count; i++)
        {
            CreateCargoGameObject(cart, i);
        }
    }

    private void SetJoint(int index, Cart cart)
    {
        cart.joint = cart.gameObject.GetComponent<ConfigurableJoint>();
        UpdateJoint(index);
        SoftJointLimit temp = new SoftJointLimit
        {
            limit = gapLength.Value
        };
        cart.joint.linearLimit = temp;
        SetJointBreakTorque(index, cart);
    }

    private void SetJointBreakTorque(int index, Cart cart)
    {
        float cartMass = cart.gameObject.GetComponent<Rigidbody>().mass;
        cart.joint.breakTorque = cartMass * (cartsList.Count - index) * breakTorque;
    }

    private void UpdateJoint(int index)
    {
        Cart cart = cartsList[index];
        if (index == 0)
        {
            cart.joint.connectedBody = locomotive.GetComponent<Rigidbody>();
        }
        else
        {
            cart.joint.connectedBody = cartsList[index - 1].gameObject.GetComponent<Rigidbody>();
            SetJointBreakTorque(index, cart);
        }
    }

    private Vector3 GetPosition(int index)
    {
        Vector3 startPosition = locomotive.transform.position - (locomotive.transform.forward.normalized * locomotiveLength.Value / 2);

        Vector3 endPosition = startPosition - (locomotive.transform.forward * ((cartLength.Value / 2) + (index * cartLength.Value) + ((index + 1) * (gapLength.Value + jointSnappingLength.Value))));

        if (linkedPath.Enabled && linkedPath.value.path != null)
        {
            float arcDistance = (startPosition - endPosition).magnitude;
            float t = linkedPath.value.path.GetClosestTimeOnPath(startPosition);

            float newT = linkedPath.value.path.GetTAfterXUnitsFromT(t, -arcDistance);
            Vector3 position = linkedPath.value.path.GetPointAtTime(newT);
            position += linkedPath.value.GetComponent<TrainRailLinker>().Height * linkedPath.value.path.GetNormal(newT);
            //Debug.DrawLine(startPosition, position);
            return position;
        }
        return endPosition;
        //return this.transform.position + (this.transform.forward * -((index * cartLength / 2) + (locomotiveLength / 2) + ((index + 1) * (gapLength + jointSnappingLength))));
    }

    private Quaternion GetRotation(int index)
    {
        if (linkedPath.Enabled && linkedPath.value.path != null)
        {
            Vector3 position = GetPosition(index);
            float t = linkedPath.value.path.GetClosestTimeOnPath(position);
            Debug.DrawLine(linkedPath.value.path.GetPointAtTime(t), position, Color.blue);
            return linkedPath.value.path.GetRotation(t);
        }
        return locomotive.transform.rotation;
    }

    private void CreateCartGameObjectModel(Cart cart, Vector3 position)
    {
        GameObject cartModelPrefab = cartPrefabs[cart.cartID];
        GameObject cartModel = PrefabUtility.InstantiatePrefab(cartModelPrefab) as GameObject;
        cartModel.transform.position = position;
        cartModel.transform.rotation = cart.gameObject.transform.rotation;
        cartModel.transform.parent = cart.gameObject.transform;
        Undo.RegisterCreatedObjectUndo(cartModel, "Created cart model");
    }

    private void CreateCargoGameObject(Cart cart, int index)
    {
        if (cart.cargoIDs.Count > 0 && (TrainCargos)cart.cargoIDs[index] != TrainCargos.None)
        {
            GameObject cargoModelPrefab = cargoPrefabs[cart.cargoIDs[index] - 1];
            GameObject cargoModel = PrefabUtility.InstantiatePrefab(cargoModelPrefab) as GameObject;

            cargoModel.transform.rotation = cart.gameObject.transform.rotation;
            cargoModel.transform.parent = cart.gameObject.transform;
            cargoModel.transform.localPosition = Vector3.zero + (Vector3.up * index * cargoHeight.Value);
            cargoModel.name = cargoModel.name + " " + index;
            Undo.RegisterCreatedObjectUndo(cargoModel, "Created cargo model");
        }
    }

    private Cart CreateNewCart(int cartID, int cargoID)
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
        UpdateJoint(indexB);

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