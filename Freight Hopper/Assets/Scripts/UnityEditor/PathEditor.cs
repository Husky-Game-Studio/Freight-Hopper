using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

/// <summary>
/// A UnityEditor script that visalizes and enables modifications to a BezierPath
/// </summary>
[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    private const float buttonWidth = 120.0f;
    private Rect windowRect = new Rect(20, 40, 10, 10); //Window for editor controls
    private PathCreator creator;
    private BezierPath path;
    private RaycastHit lastMouseRaycastHit = new RaycastHit();
    private bool newRaycastHit = false;
    private SceneView scene;
    private Vector3 sceneViewCameraPos = Vector3.zero;

    private enum Operation
    {
        Translate,
        Rotate
    }

    private Operation operation = Operation.Translate;

    private void OnEnable()
    {
        creator = (PathCreator)this.target;
        scene = EditorWindow.GetWindow<SceneView>();
        if (creator.path == null)
        {
            creator.CreatePath();
        }
        path = creator.path;
    }

    public override void OnInspectorGUI()
    {
        if (creator == null)
            return;

        base.OnInspectorGUI();
    }

    private void OnValidate()
    {
        if (path != null)
        {
            creator.focusIndex = Mathf.Clamp(creator.focusIndex, 0, path.NumAnchors - 1);
        }
    }

    private void OnSceneGUI()
    {
        if (creator == null)
            return;

        HotKeys();
        windowRect = GUILayout.Window(0, windowRect, Window, "Path Editor");
        sceneViewCameraPos = scene.camera.transform.position;
        InteractableHandles();
        Visuals();
    }

    private float HandleSize(Vector3 handlePos)
    {
        return creator.pointSize * Vector3.Distance(sceneViewCameraPos, handlePos);
    }

    private void HotKeys()
    {
        Event guiEvent = Event.current;
        if (KeyDown(guiEvent, KeyCode.Keypad1))
            ActionCreateSegmentBehind();
        else if (KeyDown(guiEvent, KeyCode.Keypad2))
            ActionRemoveAnchor();
        else if (KeyDown(guiEvent, KeyCode.Keypad3))
            ActionCreateSegmentAhead();
        else if (KeyDown(guiEvent, KeyCode.Keypad4))
            ActionDecreaseFocusIndex();
        else if (KeyDown(guiEvent, KeyCode.Keypad5))
            ActionSwitchOperation();
        else if (KeyDown(guiEvent, KeyCode.Keypad6))
            ActionIncreaseFocusIndex();

        if (KeyDown(guiEvent, KeyCode.Keypad8) && MouseRaycast(guiEvent, out lastMouseRaycastHit))
            newRaycastHit = true;
        else
            newRaycastHit = false;
    }

    //Draw a grid of buttons in a numpad layout
    private void Window(int windowID)
    {
        Color defaultColor = GUI.color;
        GUI.color = Color.red;
        GUILayout.BeginHorizontal();
        GUILayout.Button(new GUIContent(" ", "[Numpad 7]"), GUILayout.Width(buttonWidth)); //Just displaying numpad layout
        GUILayout.Button(new GUIContent("Match Pos/Rot", "[Numpad 8] (Keybind Only)"), GUILayout.Width(buttonWidth)); //Keybind only
        GUILayout.Button(new GUIContent(" ", "[Numpad 9]"), GUILayout.Width(buttonWidth)); //Just displaying numpad layout
        GUILayout.EndHorizontal();
        GUI.color = defaultColor;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Next Anchor (-)", "[Numpad 4]"), GUILayout.Width(buttonWidth)))
            ActionDecreaseFocusIndex();
        if (GUILayout.Button(new GUIContent("Switch Operation", "[Numpad 5]"), GUILayout.Width(buttonWidth)))
            ActionSwitchOperation();
        if (GUILayout.Button(new GUIContent("Next Anchor (+)", "[Numpad 6]"), GUILayout.Width(buttonWidth)))
            ActionIncreaseFocusIndex();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Add Segment (-)", "[Numpad 1]"), GUILayout.Width(buttonWidth)))
            ActionCreateSegmentBehind();
        if (GUILayout.Button(new GUIContent("Delete Anchor", "[Numpad 2]"), GUILayout.Width(buttonWidth)))
            ActionRemoveAnchor();
        if (GUILayout.Button(new GUIContent("Add Segment (+)", "[Numpad 3]"), GUILayout.Width(buttonWidth)))
            ActionCreateSegmentAhead();
        GUILayout.EndHorizontal();

        GUI.DragWindow(); //Must be at end of this script (read it's description)
    }

    private void ActionDecreaseFocusIndex()
    {
        if (creator.focusIndex > 0)
        {
            Undo.RecordObject(creator, "Change Path Focus");
            creator.focusIndex -= 1;
        }
    }

    private void ActionIncreaseFocusIndex()
    {
        if (creator.focusIndex < path.NumAnchors - 1)
        {
            Undo.RecordObject(creator, "Change Path Focus");
            creator.focusIndex += 1;
        }
    }

    private void ActionSwitchOperation()
    {
        Undo.RecordObject(creator, "Switch Path Operation");
        operation = (operation == Operation.Translate) ? Operation.Rotate : Operation.Translate;
    }

    private void ActionCreateSegmentBehind()
    {
        Undo.RecordObject(creator, "Add Segment");
        Vector3 direction = -CalculatePathDirection(creator.focusIndex);
        Vector3 origin = path[3 * creator.focusIndex];
        float distance = 0.04f * (sceneViewCameraPos - creator.transform.TransformPoint(origin)).magnitude;
        path.AddSegment(creator.focusIndex, origin + (3f * distance * direction), origin + (2 * distance * direction), origin + (1 * distance * direction));
    }

    private void ActionCreateSegmentAhead()
    {
        Undo.RecordObject(creator, "Add Segment");
        Vector3 direction = CalculatePathDirection(creator.focusIndex);
        Vector3 origin = path[3 * creator.focusIndex];
        float distance = 0.04f * (sceneViewCameraPos - creator.transform.TransformPoint(origin)).magnitude;
        path.AddSegment(creator.focusIndex + 1, origin + (1 * distance * direction), origin + (2 * distance * direction), origin + (3 * distance * direction));
        creator.focusIndex += 1;
    }

    private void ActionRemoveAnchor()
    {
        Undo.RecordObject(creator, "Remove Segment");
        path.RemoveAnchor(creator.focusIndex);
        creator.focusIndex = Mathf.Clamp(creator.focusIndex, 0, path.NumAnchors - 1);
    }

    private bool MouseRaycast(Event guiEvent, out RaycastHit raycastHit)
    {
        return Physics.Raycast(HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition), out raycastHit);
    }

    private Vector3 CalculatePathDirection(int anc)
    {
        if (0 <= anc && anc <= path.NumAnchors - 1)
        {
            if ((3 * anc) + 1 <= path.NumPoints - 1)
                return (path[(3 * creator.focusIndex) + 1] - path[3 * creator.focusIndex]).normalized;
            else
                return (path[3 * creator.focusIndex] - path[(3 * creator.focusIndex) - 1]).normalized;
        }
        else
        {
            throw new System.IndexOutOfRangeException("Anchor index out of range");
        }
    }

    private void InteractableHandles()
    {
        ClickableAnchorHandles();
        if (operation == Operation.Translate)
        {
            TranslateAnchorHandles();
        }
        else if (operation == Operation.Rotate)
        {
            RotateAnchorHandles();
        }
    }

    private void ClickableAnchorHandles()
    {
        Handles.color = Color.cyan;
        for (int i = 0; i < path.NumAnchors; i++)
        {
            if (i != creator.focusIndex) //For all anchors except the one in focus
            {
                Vector3 pos = creator.transform.TransformPoint(path.GetAnchor(i));
                if (Handles.Button(pos, Quaternion.identity, HandleSize(pos), HandleSize(pos), Handles.SphereHandleCap)) //Click SphereHandleCap to focus on clicked anchor
                {
                    creator.focusIndex = i;
                }
            }
        }
    }

    private void TranslateAnchorHandles()
    {
        bool doneTransforming = false;
        int indexCurrent = creator.focusIndex * 3;
        Vector3 anchorStoredPos = path[indexCurrent];
        Vector3 anchorHandle = Handles.PositionHandle(creator.transform.TransformPoint(anchorStoredPos), Quaternion.identity);
        Vector3 anchorHandlePos = creator.transform.InverseTransformPoint(anchorHandle);

        if (0 <= indexCurrent - 1 && indexCurrent - 1 <= path.NumPoints - 1)
        {
            Vector3 strength1StoredPos = path[indexCurrent - 1];
            Vector3 strength1Handle = Handles.PositionHandle(creator.transform.TransformPoint(strength1StoredPos), Quaternion.identity);
            Vector3 strength1HandlePos = creator.transform.InverseTransformPoint(strength1Handle);

            if (strength1StoredPos != strength1HandlePos && !doneTransforming)
            {
                Undo.RecordObject(creator, "Translate Strength Point");
                if (0 <= indexCurrent + 1 && indexCurrent + 1 <= path.NumPoints - 1)
                {
                    Vector3 direction = (path[indexCurrent] - path[indexCurrent - 1]).normalized;
                    float strength2 = (path[indexCurrent + 1] - path[indexCurrent]).magnitude;
                    path.MovePoint(indexCurrent + 1, path[indexCurrent] + (strength2 * direction));
                }
                path.MovePoint(indexCurrent - 1, strength1HandlePos);
                doneTransforming = true;
            }
        }

        if (0 <= indexCurrent + 1 && indexCurrent + 1 <= path.NumPoints - 1)
        {
            Vector3 strength2StoredPos = path[indexCurrent + 1];
            Vector3 strength2Handle = Handles.PositionHandle(creator.transform.TransformPoint(strength2StoredPos), Quaternion.identity);
            Vector3 strength2HandlePos = creator.transform.InverseTransformPoint(strength2Handle);

            if (strength2StoredPos != strength2HandlePos && !doneTransforming)
            {
                Undo.RecordObject(creator, "Translate Strength Point");
                if (0 <= indexCurrent - 1 && indexCurrent - 1 <= path.NumPoints - 1)
                {
                    Vector3 direction = (path[indexCurrent + 1] - path[indexCurrent]).normalized;
                    float strength1 = (path[indexCurrent] - path[indexCurrent - 1]).magnitude;
                    path.MovePoint(indexCurrent - 1, path[indexCurrent] - (strength1 * direction));
                }
                path.MovePoint(indexCurrent + 1, strength2HandlePos);
                doneTransforming = true;
            }
        }

        if (newRaycastHit)
            anchorHandlePos = creator.transform.InverseTransformPoint(lastMouseRaycastHit.point);
        if (anchorStoredPos != anchorHandlePos && !doneTransforming)
        {
            Undo.RecordObject(creator, "Translate Anchor Point");
            if (0 <= indexCurrent + 1 && indexCurrent + 1 <= path.NumPoints - 1)
                path.MovePoint(indexCurrent + 1, anchorHandlePos + path[indexCurrent + 1] - path[indexCurrent]);
            if (0 <= indexCurrent - 1 && indexCurrent - 1 <= path.NumPoints - 1)
                path.MovePoint(indexCurrent - 1, anchorHandlePos + (path[indexCurrent - 1] - path[indexCurrent]));
            path.MovePoint(indexCurrent, anchorHandlePos);
        }
    }

    private void RotateAnchorHandles()
    {
        Handles.color = Color.green;
        int indexCurrent = creator.focusIndex * 3;
        Vector3 anchorStoredPos = path[indexCurrent];
        Quaternion anchorStoredRot = Quaternion.LookRotation(CalculatePathDirection(creator.focusIndex));
        Quaternion anchorHandle = Handles.RotationHandle(Quaternion.LookRotation(creator.transform.TransformDirection(anchorStoredRot * Vector3.forward)), creator.transform.TransformPoint(anchorStoredPos));
        Quaternion anchorHandleRot = Quaternion.LookRotation(creator.transform.InverseTransformDirection(anchorHandle * Vector3.forward));
        if (newRaycastHit)
            anchorHandleRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(CalculatePathDirection(creator.focusIndex), lastMouseRaycastHit.normal));
        //Rotate Anchor Handle
        if (anchorStoredRot != anchorHandleRot)
        {
            Undo.RecordObject(creator, "Rotate Anchor Point");
            if (0 <= indexCurrent + 1 && indexCurrent + 1 <= path.NumPoints - 1)
                path.MovePoint(indexCurrent + 1, anchorStoredPos + (Vector3.Distance(anchorStoredPos, path[indexCurrent + 1]) * (anchorHandleRot * Vector3.forward)));
            if (0 <= indexCurrent - 1 && indexCurrent - 1 <= path.NumPoints - 1)
                path.MovePoint(indexCurrent - 1, anchorStoredPos - (Vector3.Distance(anchorStoredPos, path[indexCurrent - 1]) * (anchorHandleRot * Vector3.forward)));
        }
        //Moving strength handle ahead constrained by rotation
        if (0 <= indexCurrent + 1 && indexCurrent + 1 <= path.NumPoints - 1)
        {
            Vector3 pos = creator.transform.TransformPoint(path[indexCurrent + 1]);
            Vector3 strengthHandle = Handles.FreeMoveHandle(pos, Quaternion.identity, HandleSize(pos), Vector3.zero, Handles.SphereHandleCap);
            Vector3 strengthHandlePos = creator.transform.InverseTransformPoint(strengthHandle);
            if (path[indexCurrent + 1] != strengthHandlePos)
            {
                Undo.RecordObject(creator, "Adjust Path Strength");
                path.MovePoint(indexCurrent + 1, anchorStoredPos + (Vector3.Distance(anchorStoredPos, strengthHandlePos) * (anchorHandleRot * Vector3.forward)));
            }
        }
        //Moving strength handle behind constrained by rotation
        if (0 <= indexCurrent - 1 && indexCurrent - 1 <= path.NumPoints - 1)
        {
            Vector3 pos = creator.transform.TransformPoint(path[indexCurrent - 1]);
            Vector3 strengthHandle = Handles.FreeMoveHandle(pos, Quaternion.identity, HandleSize(pos), Vector3.zero, Handles.SphereHandleCap);
            Vector3 strengthHandlePos = creator.transform.InverseTransformPoint(strengthHandle);
            if (path[indexCurrent - 1] != strengthHandlePos)
            {
                Undo.RecordObject(creator, "Adjust Path Strength");
                path.MovePoint(indexCurrent - 1, anchorStoredPos - (Vector3.Distance(anchorStoredPos, strengthHandlePos) * (anchorHandleRot * Vector3.forward)));
            }
        }
    }

    private void Visuals()
    {
        Handles.color = Color.green;
        //Bezier Lines
        for (int i = 0; i < path.NumSegments; i++)
        {
            Vector3[] points = path.GetSegment(i);
            for (int j = 0; j < points.Length; j++)
            {
                points[j] = creator.transform.TransformPoint(points[j]);
            }
            Handles.DrawLine(points[0], points[1]);
            Handles.DrawLine(points[2], points[3]);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.white, null, 2);
        }
        //Handle for anchor focused on
        Vector3 pos = creator.transform.TransformPoint(path.GetAnchor(creator.focusIndex));
        Handles.SphereHandleCap(0, pos, Quaternion.identity, 0.3f * HandleSize(pos), EventType.Repaint);
    }

    private bool KeyDown(Event guiEvent, KeyCode key)
    {
        return guiEvent.keyCode == key && guiEvent.type == EventType.KeyDown;
    }
}

#endif