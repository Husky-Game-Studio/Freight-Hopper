using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCorrectionCollision : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Transform stepRayLower;

    [Header("Steps")]
    public float maxStepHeight = 1f;              ///< The maximum a player can set upwards in units when they hit a wall that's potentially a step
    public float stepSearchOvershoot = 0.01f;       ///< How much to overshoot into the direction a potential step in units when testing. High values prevent player from walking up small steps but may cause problems.

    private List<ContactPoint> allCPs = new List<ContactPoint>();
    private Vector3 lastVelocity;
    private Vector3 lastRayLowerPosition;

    private void Awake()
    {
        rb = Player.Instance.modules.rigidbody;
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.velocity;

        Vector3 stepUpOffset = default(Vector3);
        bool stepUp = false;

        stepUp = FindStep(out stepUpOffset, allCPs, velocity, lastVelocity);

        //Steps
        if (stepUp)
        {
            rb.MovePosition(rb.position + stepUpOffset);
            rb.velocity = lastVelocity + lastVelocity * Time.fixedDeltaTime;
        }

        allCPs.Clear();
        lastVelocity = velocity;
        lastRayLowerPosition = stepRayLower.position;
    }

    public void AddContacts(Collision col)
    {
        allCPs.AddRange(col.contacts);
    }

    /// Find the first step up point if we hit a step \param allCPs List to search \param
    /// stepUpOffset A Vector3 of the offset of the player to step up the step \return If we found a step
    private bool FindStep(out Vector3 stepUpOffset, List<ContactPoint> allCPs, Vector3 currVelocity, Vector3 lastVelocity)
    {
        stepUpOffset = default(Vector3);

        //No chance to step if the player is not moving
        Vector2 velocityXZ = new Vector2(currVelocity.x, currVelocity.z);
        if (velocityXZ.sqrMagnitude < 0.0001f)
            return false;

        foreach (ContactPoint cp in allCPs)
        {
            bool test = ResolveStepUp(out stepUpOffset, cp, lastVelocity);
            if (test)
                return test;
        }
        return false;
    }

    /// Takes a contact point that looks as though it's the side face of a step and sees if we can
    /// climb it \param stepTestCP ContactPoint to check. \param groundCP ContactPoint on the
    /// ground. \param stepUpOffset The offset from the stepTestCP.point to the stepUpPoint (to add
    /// to the player's position so they're now on the step) \return If the passed ContactPoint was
    /// a step
    private bool ResolveStepUp(out Vector3 stepUpOffset, ContactPoint stepTestCP, Vector3 lastVelocity)
    {
        stepUpOffset = default(Vector3);

        if (Mathf.Abs(stepTestCP.normal.y) >= 1 - 0.01f)
        {
            return false;
        }

        Collider stepCol = stepTestCP.otherCollider;
        RaycastHit hitInfoSide;
        Vector3 flattenedVelocity = Vector3.ProjectOnPlane(lastVelocity, rb.transform.up);

        Vector3 originSide = lastRayLowerPosition;
        Vector3 directionSide = flattenedVelocity.normalized;
        if (directionSide == Vector3.zero) { directionSide = rb.transform.forward; }
        /*Debug.DrawRay(originSide, directionSide * 5, Color.blue, 20f);
        Debug.DrawRay(originSide, -rb.transform.up * 5, Color.blue, 20f);*/
        /*Debug.DrawRay(stepTestCP.point, stepTestCP.normal * 5, Color.yellow, 20f);
        Debug.DrawRay(lastRayLowerPosition, rb.transform.up * 5, Color.green, 20f);*/
        if (!stepCol.Raycast(new Ray(originSide, directionSide), out hitInfoSide, flattenedVelocity.magnitude * Time.fixedDeltaTime + 1))
        {
            //Debug.DrawRay(originSide, directionSide * 5, Color.red, 20f);
            //Debug.DrawRay(originSide, -rb.transform.up * 5, Color.red, 20f);
            //Debug.Log("failed because didn't hit side surface");
            return false;
        }
        //( 1 ) Check if the contact point normal matches that of a step (y close to 0)
        //Debug.DrawRay(hitInfoSide.point, hitInfoSide.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
        if (Vector3.Angle(hitInfoSide.normal, rb.transform.up) < 60)
        {
            Debug.Log("failed becaue hit slope");
            return false;
        }

        //( 2 ) Make sure the contact point is low enough to be a step
        if (!(stepTestCP.point.y - lastRayLowerPosition.y < maxStepHeight))
        {
            Debug.Log("failed because step not low enough");
            return false;
        }

        //( 3 ) Check to see if there's actually a place to step in front of us
        //Fires one Raycast
        RaycastHit hitInfo;
        float stepHeight = lastRayLowerPosition.y + maxStepHeight + 0.001f;
        Vector3 stepTestInvDir = directionSide;
        Vector3 stepOutPoint = Vector3.zero/*stepTestCP.normal * -flattenedVelocity.magnitude * Time.fixedDeltaTime*/;
        Vector3 origin = new Vector3(stepTestCP.point.x, stepHeight, stepTestCP.point.z) + (stepTestInvDir * stepSearchOvershoot) + stepOutPoint;

        Debug.DrawRay(origin, stepTestInvDir * 5, Color.blue, 20f);
        Debug.DrawRay(origin, -rb.transform.up * 5, Color.blue, 20f);
        Vector3 direction = -rb.transform.up;
        if (!stepCol.Raycast(new Ray(origin, direction), out hitInfo, maxStepHeight + 0.01f))
        {
            Debug.Log("failed because couldn't find step");
            Debug.Log("failed with speed: " + lastVelocity.magnitude);
            Debug.DrawRay(origin, stepTestInvDir, Color.red, 20f);
            Debug.DrawRay(origin, -rb.transform.up * (maxStepHeight + 0.01f), Color.red, 20f);
            return false;
        }
        //Debug.Log("passed with origin of " + origin);
        //We have enough info to calculate the points
        Vector3 difContactPoint = new Vector3(stepTestCP.point.x, 0, stepTestCP.point.z) - new Vector3(lastRayLowerPosition.x, 0, lastRayLowerPosition.z);
        Vector3 stepUpPointOffset = new Vector3(0, hitInfo.point.y - lastRayLowerPosition.y, 0) + difContactPoint;
        //We passed all the checks! Calculate and return the point!
        stepUpOffset = stepUpPointOffset;
        Debug.Log("edge correction");
        return true;
    }
}