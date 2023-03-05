using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private Memory<Vector3> position;
    [SerializeField] private Transform head;
    [SerializeField] private MeshRenderer playerMeshRenderer;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask deathMask;
    private bool isEnabled = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(head.position, radius);
    }

    private void Awake()
    {
        isEnabled = false;
        Player.PlayerCanMove += EnableCollision;
    }
    private void OnDestroy()
    {
        Player.PlayerCanMove -= EnableCollision;
    }
    void EnableCollision(){
        position.SetCurrent(head.transform.position);
        position.UpdateOld();
        isEnabled = true;
    }
    private void CheckInBetweenCollision()
    {
        position.SetCurrent(head.transform.position);
        Vector3 dif = position.current - position.old;
        float dist = dif.magnitude;
        Vector3 dir = dif.normalized;
        // Inbetween fixed updates, the player has essentially teleported. This sphere cast will
        // check inbetween the teleportations in a radius to see if we have hit anything
        if (Physics.SphereCast(position.old, radius, dir, out _, dist, deathMask))
        {
            head.parent.transform.position = position.old; // This is to move the camera back to the old position so that its not inside walls (as much)
            playerMeshRenderer.enabled = false; // If we don't do this then the player sees themself when they die. It looks odd
            KillPlayer();
        }
        position.UpdateOld();
    }

    private void FixedUpdate()
    {
        if (!isEnabled)
        {
            return;
        }
        CheckInBetweenCollision();
    }

    public void KillPlayer()
    {
        if (Goal.CompletedLevel)
        {
            return;
        }
        LevelController.Instance.Respawn();
    }
}