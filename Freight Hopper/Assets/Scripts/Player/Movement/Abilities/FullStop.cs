using UnityEngine;

[System.Serializable]
public class FullStop
{
    [SerializeField, ReadOnly] private bool fullStopPossible = true;
    [SerializeField] public Timer cooldown = new Timer(2);

    private Rigidbody rb;

    public void Initialize(Rigidbody rb)
    {
        this.rb = rb;
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