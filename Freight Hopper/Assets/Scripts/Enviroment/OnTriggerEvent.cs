using UnityEngine;
using System;

public class OnTriggerEvent : MonoBehaviour
{
    public Action triggered;

    [SerializeField] private bool triggerOnPlayerEnter;
    [SerializeField] private Optional<GameObject> triggerOnGameObjectEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnGameObjectEnter.Enabled && triggerOnGameObjectEnter.value != null && other.gameObject == triggerOnGameObjectEnter.value)
        {
            triggered?.Invoke();
        }
        if (triggerOnPlayerEnter && other.CompareTag("Player"))
        {
            triggered?.Invoke();
        }
    }
}