using UnityEngine;

public class DistantRigidbodyLinker : MonoBehaviour
{
    [SerializeField] private Vector3 pointOne;
    [SerializeField] private Vector3 pointTwo;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layerMask;
    private Rigidbody rb;
    private RigidbodyLinker rigibodyLinker;
    private CollisionManagement collisionManagent;
    Collider[] results = new Collider[10];

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.TransformPoint(pointOne), radius);
        Gizmos.DrawWireSphere(this.transform.TransformPoint(pointTwo), radius);
    }

    private void Awake()
    {
        rb = Player.Instance.modules.rigidbody;
        rigibodyLinker = Player.Instance.modules.rigidbodyLinker;
        collisionManagent = Player.Instance.modules.collisionManagement;
    }

    private void FixedUpdate()
    {
        EmitSkin();
    }

    private void EmitSkin()
    {
        var size = Physics.OverlapCapsuleNonAlloc(this.transform.TransformPoint(pointOne), this.transform.TransformPoint(pointTwo), radius, results, layerMask);
        for (int i = 0; i < size; i++)
        {
            if (results[i].attachedRigidbody != null)
            {
                rigibodyLinker.UpdateLink(results[i].attachedRigidbody);
            }
        }
    }
}