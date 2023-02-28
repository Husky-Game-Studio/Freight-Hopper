using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    [SerializeField] Collider capsuleCollider; 
    private void OnEnable()
    {
        capsuleCollider.enabled = Settings.GetIsPlayerCollisionEnabled;
    }
}
