using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    private void Start()
    {
        this.gameObject.SetActive(false);
    }
}