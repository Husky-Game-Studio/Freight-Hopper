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
        EditorGUILayout.LabelField("LaserGrid Settings");
       
        //string[] cargoNames = System.Enum.GetNames(typeof(TrainBuilder.TrainCargos));
        
        //EditorGUILayout.Popup("Carts", cartIndex, laserGridSettings);
        
        
        if (GUILayout.Button("Generate"))
        {
            laserGridBuilder.BuildLaserGrid();
        }
        base.OnInspectorGUI();
    }
}

#endif