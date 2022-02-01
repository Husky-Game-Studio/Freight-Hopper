using UnityEngine;

[System.Serializable]
public class RigidbodyLinker : MonoBehaviour
{
    [ReadOnly, SerializeField] private Memory<Rigidbody> connectedRb = new Memory<Rigidbody>(null, null);
    [ReadOnly, SerializeField] private Memory<Vector3> connectionVelocity;
    [ReadOnly, SerializeField] private Memory<Vector3> connectionAcceleration;

    public Memory<Rigidbody> ConnectedRb => connectedRb;
    public Memory<Vector3> ConnectionAcceleration => connectionAcceleration;
    public Memory<Vector3> ConnectionVelocity => connectionVelocity;

    /// <summary>
    /// Assigns connected rigidbody to parameter
    /// </summary>
    public void UpdateLink(Rigidbody rigidbody)
    {
        if (rigidbody == null)
        {
            return;
        }
        connectedRb.current = rigidbody;
    }

    // Applies acceleration to ourRigidbody if applicable. Not applicable if mass of linked rigidbody
    // is too small and is not kinematic
    public void UpdateConnectionState(Rigidbody ourRigidbody)
    {
        if (connectedRb.current == null)
        {
            return;
        }

        if (!connectedRb.current.isKinematic && connectedRb.current.mass < ourRigidbody.mass)
        {
            return;
        }

        connectionVelocity.current = connectedRb.current.GetPointVelocity(ourRigidbody.position);
        if (connectedRb.current == connectedRb.old)
        {
            connectionAcceleration.current = connectionVelocity.current - connectionVelocity.old;
            ourRigidbody.AddForce(connectionAcceleration.current, ForceMode.VelocityChange);
        }
    }

    public void UpdateOldValues()
    {
        connectionAcceleration.UpdateOld();
        connectionVelocity.UpdateOld();
        connectedRb.UpdateOld();
    }

    public void ClearValues()
    {
        connectionAcceleration.current = Vector3.zero;
        connectionVelocity.current = Vector3.zero;

        connectedRb.current = null;
    }
}