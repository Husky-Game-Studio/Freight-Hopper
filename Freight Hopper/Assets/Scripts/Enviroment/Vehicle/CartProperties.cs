using UnityEngine;

public class CartProperties : MonoBehaviour
{
    [SerializeField] private int indexOfCart;
    private ConfigurableJoint joint;
    public int IndexOfCart => indexOfCart;

    public void SetCartIndex(int index)
    {
        indexOfCart = index;
        joint = GetComponent<ConfigurableJoint>();
    }

    public void SnapJoint()
    {
        if (joint != null)
        {
            Destroy(joint);
        }
    }
}