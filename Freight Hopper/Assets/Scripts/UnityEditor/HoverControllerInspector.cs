using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(HoverController))]
public class HoverControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Add Hover Engine"))
        {
            HoverController controller = (HoverController)this.target;
            controller.AddHoverEngine(Vector3.zero, this.name);
        }
        base.OnInspectorGUI();
    }
}

#endif