using UnityEngine;

[System.Serializable]
public class FullStop : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool fullStopPossible = true;
    [SerializeField] public Timer cooldown = new Timer(2);

    private Rigidbody rb;
    private CollisionManagement playerCollision;

    public void Initialize(Rigidbody rb, CollisionManagement playerCollision)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
    }

    private void OnEnable()
    {
        UserInput.Input.FullStopInput += TryFullStop;
        playerCollision.Landed += Landed;
        playerCollision.CollisionDataCollected += cooldown.CountDownFixed;
    }

    private void OnDisable()
    {
        UserInput.Input.FullStopInput -= TryFullStop;
        playerCollision.Landed -= Landed;
        playerCollision.CollisionDataCollected -= cooldown.CountDownFixed;
    }

    public void Landed()
    {
        fullStopPossible = true;
    }

    public void TryFullStop()
    {
        // Stops the player
        if (!cooldown.TimerActive() && fullStopPossible)
        {
            cooldown.ResetTimer();
            rb.velocity = Vector3.zero;
            fullStopPossible = false;
        }
    }
}