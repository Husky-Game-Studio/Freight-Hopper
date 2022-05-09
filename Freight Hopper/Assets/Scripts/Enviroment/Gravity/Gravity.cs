using UnityEngine;

[System.Serializable]
public class Gravity : MonoBehaviour
{
    [SerializeField] private bool useGravity = true;
    [SerializeField] private bool automatic = true;
    [SerializeField, ReadOnly] private Rigidbody rb;

    private void Awake()
    {
        this.rb = GetComponentInParent<Rigidbody>();
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (automatic)
        {
            GravityLoop();
        }
    }

    public void EnableGravity(bool enable = true)
    {
        useGravity = enable;
    }

    public void GravityLoop()
    {
        if (!useGravity)
        {
            return;
        }
        rb.AddForce(CustomGravity.GetGravity(), ForceMode.Acceleration);
    }
}