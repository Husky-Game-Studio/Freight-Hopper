using UnityEngine;

public class CartProperties : MonoBehaviour
{
    [SerializeField] private int indexOfCart;
    private ConfigurableJoint joint;
    private bool destroying = false;
    private bool hadJoint = false;

    public bool HadJoint => hadJoint;
    public bool JointSnapped => joint == null || joint.connectedBody == null;
    public int IndexOfCart => indexOfCart;

    public void SetCartIndex(int index)
    {
        indexOfCart = index;
    }

    private void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();
        if (joint != null)
        {
            hadJoint = true;
        }
    }

    public void SnapJoint()
    {
        if (joint != null)
        {
            Destroy(joint);
        }
    }

    private void FixedUpdate()
    {
        if (this.JointSnapped && this.HadJoint && !destroying)
        {
            GetComponent<Destructable>().DestroyObject();
            destroying = true;
        }
    }
}