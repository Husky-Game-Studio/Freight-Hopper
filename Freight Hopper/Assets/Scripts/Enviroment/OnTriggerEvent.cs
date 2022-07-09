using UnityEngine;
using System;
using System.Collections.Generic;

public class OnTriggerEvent : MonoBehaviour
{
    public Action triggered;
    public static bool staticTriggered;

    [SerializeField] private bool triggerOnPlayerEnter;
    // GameObject MUST have a collider component
    [SerializeField] private Optional<List<GameObject>> triggerOnGameObjectEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnGameObjectEnter.Enabled)
        {
            for (int i = 0; i < triggerOnGameObjectEnter.value.Count; i++)
            {
                if (triggerOnGameObjectEnter.value
                    != null && other.gameObject == triggerOnGameObjectEnter.value[i])
                {
                    triggered?.Invoke();
                    staticTriggered = true;
                }
            }
            return;
        }

        if (triggerOnPlayerEnter && other.CompareTag("Player"))
        {
            triggered?.Invoke();
            staticTriggered = true;
            return;
        }

        triggered?.Invoke();
        staticTriggered = true;
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