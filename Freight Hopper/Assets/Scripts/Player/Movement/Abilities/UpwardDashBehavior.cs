using UnityEngine;

public class UpwardDashBehavior : AbilityBehavior
{
    [SerializeField, ReadOnly] private Vector3 storedVelocity;

    [SerializeField] private float initialUpwardsForce;
    [SerializeField] private float consistentForce;
    [SerializeField] private float newTimeScale;

    public Timer duration = new Timer(1);
    private CollisionManagement collisionManager;
    Vector3 directionEntryVector;
    public override void Initialize()
    {
        base.Initialize();
        collisionManager = Player.Instance.modules.collisionManagement;
    }
    private void Awake()
    {
    }
    public override void EntryAction()
    {
        storedVelocity = rb.velocity;
        duration.ResetTimer();
        soundManager.Play("UpwardDashEntry");
        Time.timeScale = newTimeScale;
        //rb.velocity = Vector3.zero;
        //directionEntryVector = Camera.main.transform.forward;
        //rb.AddForce(directionEntryVector * initialUpwardsForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        soundManager.Play("UpwardDashTick");
        duration.CountDown(Time.fixedUnscaledDeltaTime);
        
        //rb.AddForce(directionEntryVector * consistentForce, ForceMode.VelocityChange);
        //rb.velocity = Vector3.Project(rb.velocity, directionEntryVector);
    }

    public override void ExitAction()
    {
        base.ExitAction();
        soundManager.Stop("UpwardDashTick");
        soundManager.Play("UpwardDashExit");
        Time.timeScale = 1f;
        rb.velocity = storedVelocity.magnitude * Camera.main.transform.forward;
        //storedVelocity = Vector3.zero;
    }
}