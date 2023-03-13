using UnityEngine;

[System.Serializable]
public class Gravity : MonoBehaviour
{
    [SerializeField] private bool useGravity = true;
    [SerializeField] private bool automatic = true;
    [SerializeField, ReadOnly] private Rigidbody rb;
    private float gravityScale = 1;


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

    public void SetGravityScale(float val) => gravityScale = val;
    public void ResetGravityScale() => gravityScale = 1;

    public void EnableGravity(bool enable = true)
    {
        useGravity = enable;
    }

    public void GravityLoop()
    {
        if (!useGravity || Time.timeScale == 0)
        {
            return;
        }
        rb.AddForce(CustomGravity.GetGravity() * gravityScale, ForceMode.Acceleration);
    }
}