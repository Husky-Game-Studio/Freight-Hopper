using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoleBehavior : MonoBehaviour
{
    [SerializeField, ReadOnly] private float length;
    [SerializeField] private float maxLength = 10;
    [SerializeField] private float grappleExtensionSpeed = 10;

    //[SerializeField] private float anchoredGrappleSpeed = 10;
    [SerializeField] private LayerMask affectedLayers;

    private Vector3 inputDirection;

    private Ray playerAnchor;
    private Ray anchor;
    private Rigidbody anchoredRb;
    private Vector3 anchoredRbLocalPosition;
    private bool anchored;

    private Rigidbody rb;
    private CollisionManagement playerCollision;
    private Transform cameraTransform;

    public void Initialize(Rigidbody rb, CollisionManagement playerCollision)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
        this.cameraTransform = Camera.main.transform;
    }

    public void Grapple(/*Vector3 direction*/)
    {
        //Debug.Log("Grappling");

        if (anchoredRb != null)
        {
            anchor = new Ray(anchoredRb.transform.TransformPoint(anchoredRbLocalPosition), -playerAnchor.direction);
        }
        playerAnchor = new Ray(cameraTransform.position, playerAnchor.direction);
        Debug.DrawLine(playerAnchor.origin, anchor.origin, Color.red, 1);
    }

    public void GrappleTransition()
    {
        //Debug.Log("Grapple Firing");
        playerAnchor = new Ray(cameraTransform.position, cameraTransform.transform.forward);

        length += Time.fixedDeltaTime * grappleExtensionSpeed;
        length = Mathf.Min(length, maxLength);
        RaycastHit hit;
        Debug.DrawLine(playerAnchor.origin, playerAnchor.GetPoint(length), Color.yellow, 1);
        if (Physics.Raycast(playerAnchor, out hit, length, affectedLayers))
        {
            anchored = true;
            anchor = new Ray(hit.point, -playerAnchor.direction);
            if (hit.rigidbody != null)
            {
                anchoredRb = hit.rigidbody;
                anchoredRbLocalPosition = rb.transform.InverseTransformPoint(hit.point);
            }
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
        playerAnchor = new Ray(cameraTransform.position, cameraTransform.transform.forward);
        //Debug.Log("starting grapple at: " + playerAnchor.origin + " heading to " + playerAnchor.direction);
        anchored = false;
    }

    public bool ReachedMaxDistance()
    {
        return length >= maxLength;
    }
}