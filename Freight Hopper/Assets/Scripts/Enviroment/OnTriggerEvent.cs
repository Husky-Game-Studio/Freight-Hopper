using UnityEngine;
using System;

public class OnTriggerEvent : MonoBehaviour
{
    public Action triggered;
    public static bool staticTriggered;

    [SerializeField] private bool triggerOnPlayerEnter;
    // GameObject MUST have a collider component
    [SerializeField] private Optional<GameObject> triggerOnGameObjectEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnGameObjectEnter.Enabled && triggerOnGameObjectEnter.value != null && other.gameObject == triggerOnGameObjectEnter.value)
        {
            triggered?.Invoke();
            staticTriggered = true;
        }
        if (triggerOnPlayerEnter && other.CompareTag("Player"))
        {
            triggered?.Invoke();
            staticTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        staticTriggered = false;
    }

    private void OnTriggerStay(Collider other)
    {
        staticTriggered = false;
    }
}