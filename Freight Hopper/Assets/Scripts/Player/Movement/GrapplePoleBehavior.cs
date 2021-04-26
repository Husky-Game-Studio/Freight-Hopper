using UnityEngine;

public class GrapplePoleBehavior : MonoBehaviour
{
    [SerializeField, ReadOnly] private float length;
    [SerializeField] private LineRenderer pole;
    [SerializeField, Range(0.9f, 3)] private float breakMaxLengthPercent = 1.2f;
    [SerializeField] private float maxLength = 10;
    [SerializeField] private float grappleExtensionSpeed = 10;
    [SerializeField] private float grappleMoveSpeed = 10;

    [SerializeField] private LayerMask affectedLayers;

    private Ray playerAnchor;
    private Ray anchor;
    private Rigidbody anchoredRb;
    private Vector3 anchoredRbLocalPosition;
    private bool anchored;

    [SerializeField]
    private PID distanceController;

    private Rigidbody rb;
    private CollisionManagement playerCollision;
    private Transform cameraTransform;

    public void Initialize(Rigidbody rb, CollisionManagement playerCollision)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
        this.cameraTransform = Camera.main.transform;
    }

    private void Awake()
    {
        EndGrapple();
    }

    public void Grapple(Vector3 direction)
    {
        if (anchoredRb != null)
        {
            anchor = new Ray(anchoredRb.transform.TransformPoint(anchoredRbLocalPosition), -playerAnchor.direction);
        }
        playerAnchor = new Ray(rb.position + pole.GetPosition(0), playerAnchor.direction);
        pole.SetPosition(1, rb.transform.InverseTransformPoint(anchor.origin));

        Vector3 normal = (playerAnchor.origin - anchor.origin).normalized;

        //Transform playerTransform = rb.transform;
        Vector3 tangent = Vector3.Cross(cameraTransform.right, normal);
        Vector3 bitangent = Vector3.Cross(normal, tangent);
        Debug.DrawLine(playerAnchor.origin, playerAnchor.origin + normal, Color.green, Time.fixedDeltaTime);
        Debug.DrawLine(playerAnchor.origin, playerAnchor.origin + tangent, Color.blue, Time.fixedDeltaTime);
        Debug.DrawLine(playerAnchor.origin, playerAnchor.origin + bitangent, Color.red, Time.fixedDeltaTime);

        Vector3 move = tangent * direction.z + bitangent * direction.x;
        move.Normalize();

        rb.AddForce(move * grappleMoveSpeed, ForceMode.Acceleration);

        //Vector3 applicableVelocity = Vector3.Project(rb.velocity, normal);

        float expectedLength = length;
        float actualLength = (playerAnchor.origin - anchor.origin).magnitude;
        float error = (expectedLength - actualLength);
        Vector3 tensionVelocity = normal * distanceController.GetOutput(error, Time.fixedDeltaTime);
        rb.AddForce(tensionVelocity, ForceMode.VelocityChange);
    }

    public void GrappleTransition()
    {
        playerAnchor = new Ray(rb.position + pole.GetPosition(0), cameraTransform.transform.forward);

        length += Time.fixedDeltaTime * grappleExtensionSpeed;
        length = Mathf.Min(length, maxLength);
        pole.SetPosition(1, rb.transform.InverseTransformPoint(playerAnchor.GetPoint(length)));
        RaycastHit hit;
        Debug.DrawLine(playerAnchor.origin, playerAnchor.GetPoint(length), Color.yellow, Time.fixedDeltaTime);
        if (Physics.Raycast(playerAnchor, out hit, length, affectedLayers))
        {
            anchored = true;
            anchor = new Ray(hit.point, -playerAnchor.direction);
            length = (hit.point - playerAnchor.origin).magnitude;
            if (hit.rigidbody != null)
            {
                anchoredRb = hit.rigidbody;
                anchoredRbLocalPosition = anchoredRb.transform.InverseTransformPoint(hit.point);
            }
            distanceController.Reset();
        }
    }

    public bool Anchored()
    {
        return anchored;
    }

    public void StartGrapple()
    {
        length = 0;
        anchoredRb = null;
        anchoredRbLocalPosition = Vector3.zero;
        playerAnchor = new Ray(rb.position + pole.GetPosition(0), cameraTransform.transform.forward);
        anchored = false;

        pole.enabled = true;
        pole.SetPosition(1, pole.GetPosition(0));
    }

    public void EndGrapple()
    {
        pole.enabled = false;
    }

    public bool ReachedMaxDistance()
    {
        return length >= maxLength;
    }

    public bool GrapplePoleBroken()
    {
        float actualLength = (playerAnchor.origin - anchor.origin).magnitude;
        return actualLength > maxLength * breakMaxLengthPercent;
    }
}