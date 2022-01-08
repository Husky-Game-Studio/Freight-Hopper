using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event System.Action OnValuesUpdated;

    public bool autoUpdate;
#if UNITY_EDITOR

    private void OnValidate()
    {
        if (autoUpdate)
        {
            UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }
    }

    public void NotifyOfUpdatedValues()
    {
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }

#endif
}