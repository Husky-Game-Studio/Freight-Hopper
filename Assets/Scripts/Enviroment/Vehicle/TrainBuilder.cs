using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class TrainBuilder : MonoBehaviour
{
    [SerializeField] private bool linkToPath = true;
    private PathCreation.PathCreator LinkedPath => trainFSM.pathObjects[0];

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
    [SerializeField] private PrefabsList cartPrefabs;
    [SerializeField] private PrefabsList cargoPrefabs;
    [SerializeField] private GameObject baseCart;
    // Super comments
    private TrainMachineCenter trainFSM;
    private GameObject locomotive;
    private int cartIndexSelection;
    private int cargoIndexSelection;
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

    private void Awake()
    {
        Destroy(this);
    }

    // This order matters, if this is different from the model order for the list this WON'T work
    public enum TrainCargos
    {
        None = 0,
        BoxCargo = 1,
        PillCargo = 2,
        BillBoardUp = 3,
        BillBoardDown = 4,
        BillBoardRF = 5,
        BillBoardLF = 6,
        VoidCargo = 7
    }

    // This order matters, if this is different from the model order for the list this WON'T work
    public enum TrainCarts
    {
        EmptyCart = 0,
        EmptyCartWalls = 1,
        EmptyCartLasers = 2,
        VoidCart = 3,
        NULL2 = 4,
        NULL3 = 5,
        NULL4 = 6,
        TurretCart = 7
    }

    [System.Serializable]
    private class Cart
    {
        public CartProperties cartProperties;
        public int cartID;
        public List<int> cargoIDs = new List<int>();
        public GameObject gameObject;
        public GameObject model;
        public ConfigurableJoint joint;
    }

#if UNITY_EDITOR

    public void IncrementActionIndex()
    {
        if (actionIndex == -1)
        {
            return;
        }
        else
        {
            actionIndex++;
        }
        
        if (actionIndex >= cartsList.Count)
        {
            actionIndex = -1;
        }
        SceneView.RepaintAll();
    }

    public void DecrementActionIndex()
    {
        if (actionIndex == 0) return;
        
        if (actionIndex == -1)
        {
            actionIndex = cartsList.Count - 1;
        } else {
            actionIndex--;
        }
        SceneView.RepaintAll();
    }

    public bool CanRemoveOrClear => cartsList.Count > 0;
    public bool SelectedCartHasCargo => this.ActionIndex != -1 && cartsList[this.ActionIndex].cargoIDs.Count > 0;

    // Draws a sphere where the action index is
    public void OnDrawGizmosSelected()
    {
        if (cartsList.Count < 1)
        {
            return;
        }
        Vector3 position = GetPosition(this.ActionIndex);
        if (actionIndex != -1)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(position, 8);
    }

    public void OnValidate()
    {
        trainFSM = GetComponent<TrainMachineCenter>();
        locomotive = this.transform.GetChild(0).gameObject;
        if (trainFSM == null || trainFSM.pathObjects.Count < 1 || trainFSM.pathObjects[0] == null)
        {
            return;
        }
        if (actionIndex >= cartsList.Count)
        {
            actionIndex = cartsList.Count - 1;
        }
        if (this.LinkedPath != null && linkToPath && this.LinkedPath.path != null)
        {
            var position1 = locomotive.transform.position;
            float t = this.LinkedPath.path.GetClosestTimeOnPath(position1);
            Vector3 position = this.LinkedPath.path.GetPointAtTime(t);
            position1 = position;
            float offsetDistance = this.LinkedPath.GetComponent<TrainRailLinker>().Height;
            position1 += this.LinkedPath.path.GetNormal(t) * offsetDistance;
            locomotive.transform.position = position1;
            locomotive.transform.rotation = this.LinkedPath.path.GetRotation(t);
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
            AddCartInternal(cartID, new List<int>() { cargoID });
        }
        UpdateTrain();
    }

    private void AddCartInternal(int cartId, List<int> cargoIDs)
    {
        Cart cart = CreateNewCart(cartId, cargoIDs);
        int index = this.ActionIndex;

        cartsList.Add(cart);

        UpdateCart(cartsList.Count - 1, cart);
        Undo.RegisterCreatedObjectUndo(cart.gameObject, "Created cargo gameobject");
        if (actionIndex != -1)
        {
            for (int j = cartsList.Count - 1; j > index; j--)
            {
                SwapCarts(cartsList[j], cartsList[j - 1]);
            }
            UpdateJoint(index + 1);
        }
    }
    public void Duplicate()
    {
        if (this.ActionIndex <= -1) return;
        Undo.RegisterCompleteObjectUndo(this, "Duplicate Cart");

        Cart current = cartsList[this.ActionIndex];

        for (int i = 0; i < this.repeatActionCount; i++)
        {
            AddCartInternal(current.cartID, current.cargoIDs);
        }
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

            Undo.RegisterCreatedObjectUndo(CreateCargoGameObject(cart, cart.cargoIDs.Count - 1), "Created cargo model");
        }
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

    public void SwapForward(){
        if (this.ActionIndex <= 0) return;
        Undo.RegisterCompleteObjectUndo(this, "Swap Forwards");

        int current = this.ActionIndex;
        int next = this.ActionIndex - 1;

        SwapCarts(cartsList[current], cartsList[next]);
        this.DecrementActionIndex();
    }
    public void SwapBackward()
    {
        if (this.ActionIndex == cartsList.Count - 1 || this.ActionIndex <= -1) return;
        Undo.RegisterCompleteObjectUndo(this, "Swap Backwards");

        int current = this.ActionIndex;
        int next = this.ActionIndex + 1;

        SwapCarts(cartsList[current], cartsList[next]);
        this.IncrementActionIndex();
    }

    public void ReplaceCart(int cartID, int cargoID)
    {
        Undo.RegisterCompleteObjectUndo(this, "Replace Cart");
        Cart cart = cartsList[this.ActionIndex];
        cart.cartID = cartID;
        cart.cargoIDs.Clear();
        cart.cargoIDs.Add(cargoID);

        UpdateCart(this.ActionIndex, cart);
        Undo.RegisterCreatedObjectUndo(cart.model, "Created cargo gameobject");
        if (this.ActionIndex + 1 < cartsList.Count)
        {
            UpdateJoint(this.ActionIndex + 1);
        }
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
                DestroyImmediate(child);
            }
        }
    }

    // This will clear the cart, and then rebuild it deleting everything
    [ContextMenu("Complete Update")]
    public void CompleteUpdate()
    {
        for (int i = 0; i < cartsList.Count; i++)
        {
            UpdateCart(i, cartsList[i], true);
        }
    }

    [ContextMenu("Complete Update All")]
    public void CompleteUpdateAll()
    {
        TrainBuilder[] trainBuilders = FindObjectsOfType<TrainBuilder>();
        foreach (TrainBuilder builder in trainBuilders)
        {
            builder.CompleteUpdate();
        }
    }

    private void UpdateCart(int index, Cart cart, bool completeUpdate = false)
    {
        Vector3 position = GetPosition(index);
        Quaternion rotation = GetRotation(index);
        if (cart.gameObject == null)
        {
            cart.gameObject = PrefabUtility.InstantiatePrefab(baseCart, this.transform) as GameObject;
        }
        else if (completeUpdate)
        {
            Undo.DestroyObjectImmediate(cart.gameObject);

            cart.gameObject = PrefabUtility.InstantiatePrefab(baseCart, this.transform) as GameObject;
        }

        cart.gameObject.transform.SetPositionAndRotation(position, rotation);
        cart.gameObject.name = "Cart " + index;

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
        var forward = locomotive.transform.forward;
        Vector3 startPosition = locomotive.transform.position - (forward * locomotiveLength.Value/2);

        Vector3 endPosition = startPosition - (forward * ((cartLength.Value / 2) + (index * cartLength.Value) + ((index + 1) * (gapLength.Value + jointSnappingLength.Value))));

        if (linkToPath && this.LinkedPath.path != null)
        {
            float arcDistance = (startPosition - endPosition).magnitude;
            float t = this.LinkedPath.path.GetClosestTimeOnPath(startPosition);

            float newT = this.LinkedPath.path.GetTAfterXUnitsFromT(t, -arcDistance);
            Vector3 position = this.LinkedPath.path.GetPointAtTime(newT);
            position += this.LinkedPath.GetComponent<TrainRailLinker>().Height * this.LinkedPath.path.GetNormal(newT);
            //Debug.DrawLine(startPosition, position);
            return position;
        }
        return endPosition;
        //return this.transform.position + (this.transform.forward * -((index * cartLength / 2) + (locomotiveLength / 2) + ((index + 1) * (gapLength + jointSnappingLength))));
    }

    private Quaternion GetRotation(int index)
    {
        if (linkToPath && this.LinkedPath.path != null)
        {
            Vector3 position = GetPosition(index);
            float t = this.LinkedPath.path.GetClosestTimeOnPath(position);
            Debug.DrawLine(this.LinkedPath.path.GetPointAtTime(t), position, Color.blue);
            return this.LinkedPath.path.GetRotation(t);
        }
        return locomotive.transform.rotation;
    }

    private void CreateCartGameObjectModel(Cart cart, Vector3 position)
    {
        if (cart.model != null)
        {
            Undo.DestroyObjectImmediate(cart.model);
        }
        GameObject cartModelPrefab = cartPrefabs.Prefabs[cart.cartID];
        GameObject cartModel = PrefabUtility.InstantiatePrefab(cartModelPrefab) as GameObject;
        cartModel.transform.SetPositionAndRotation(position, cart.gameObject.transform.rotation);
        cartModel.transform.parent = cart.gameObject.transform;

        cart.model = cartModel;
    }

    private GameObject CreateCargoGameObject(Cart cart, int index)
    {
        if (cart.cargoIDs.Count > 0 && (TrainCargos)cart.cargoIDs[index] != TrainCargos.None)
        {
            GameObject cargoModelPrefab = cargoPrefabs.Prefabs[cart.cargoIDs[index] - 1];
            GameObject cargoModel = PrefabUtility.InstantiatePrefab(cargoModelPrefab) as GameObject;

            cargoModel.transform.rotation = cart.gameObject.transform.rotation;
            cargoModel.transform.parent = cart.model.transform;
            cargoModel.transform.localPosition = Vector3.zero + (cargoHeight.Value * index * Vector3.up);
            cargoModel.name = cargoModel.name + " " + index;
            return cargoModel;
        }
        return null;
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
    private Cart CreateNewCart(int cartID, List<int> cargoIDs){
        Cart cart = new Cart
        {
            cartID = cartID,
        };
        cart.cargoIDs.AddRange(cargoIDs);
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

        GameObject tempModel = a.model;
        a.model = b.model;
        b.model = tempModel;
    }

#endif
}