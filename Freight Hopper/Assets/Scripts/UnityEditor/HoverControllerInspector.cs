using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HoverController))]
public class HoverControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Add Hover Engine"))
        {
            HoverController controller = (HoverController)target;
            controller.AddHoverEngine(Vector3.zero, name);
        }
        base.OnInspectorGUI();
    }
}