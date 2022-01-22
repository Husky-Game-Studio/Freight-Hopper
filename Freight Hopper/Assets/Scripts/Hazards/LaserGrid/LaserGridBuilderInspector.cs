using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(LaserGridBuilder))]
public class LaserGridBuilderInspector : Editor
{
    private LaserGridBuilder laserGridBuilder;
    
    public void OnEnable()
    {
        laserGridBuilder = (LaserGridBuilder)this.target;
        
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            laserGridBuilder.BuildLaserGrid();
        }
        
        EditorGUILayout.LabelField("LaserGrid Settings");
        base.OnInspectorGUI();
    }
}

#endif