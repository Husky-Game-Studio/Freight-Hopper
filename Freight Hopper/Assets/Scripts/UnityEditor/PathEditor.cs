using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    Rect windowRect = new Rect(20,40,10,10); //Window for editor controls
    PathCreator creator;
    BezierPath path;
    RaycastHit lastMouseRaycastHit = new RaycastHit();
    bool newRaycastHit = false;
    

    enum Operation
    {
        Translate,
        Rotate
    }
    Operation operation = Operation.Translate;

    

    private void OnEnable()
    {
        creator = (PathCreator)target;
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

    private void OnSceneGUI()
    {
        if (creator == null)
            return;

        HotKeys();
        windowRect = GUILayout.Window(0, windowRect, Window, "Path Editor");
        InteractableHandles();
        Visuals();
    }

    void HotKeys()
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

    void Window(int windowID)
    {
        Color defaultColor = GUI.color;
        GUI.color = Color.red;
        GUILayout.BeginHorizontal();
        GUILayout.Button(new GUIContent(" ", "[Numpad 7]"), GUILayout.Width(120.0f)); //Just displaying numpad layout
        GUILayout.Button(new GUIContent("Match Pos/Rot", "[Numpad 8] (Keybind Only)"), GUILayout.Width(120.0f)); //Keybind only
        GUILayout.Button(new GUIContent(" ", "[Numpad 9]"), GUILayout.Width(120.0f)); //Just displaying numpad layout
        GUILayout.EndHorizontal();
        GUI.color = defaultColor;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Next Anchor (-)", "[Numpad 4]"), GUILayout.Width(120.0f)))
            ActionDecreaseFocusIndex();
        if (GUILayout.Button(new GUIContent("Switch Operation", "[Numpad 5]"), GUILayout.Width(120.0f)))
            ActionSwitchOperation();
        if (GUILayout.Button(new GUIContent("Next Anchor (+)", "[Numpad 6]"), GUILayout.Width(120.0f)))
            ActionIncreaseFocusIndex();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Add Segment (-)", "[Numpad 1]"), GUILayout.Width(120.0f)))
            ActionCreateSegmentBehind();
        if (GUILayout.Button(new GUIContent("Delete Anchor", "[Numpad 2]"), GUILayout.Width(120.0f)))
            ActionRemoveAnchor();
        if (GUILayout.Button(new GUIContent("Add Segment (+)", "[Numpad 3]"), GUILayout.Width(120.0f)))
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
        path.AddSegment(creator.focusIndex, origin + 3 * direction, origin + 2 * direction, origin + 1 * direction);
    }

    private void ActionCreateSegmentAhead()
    {
        Undo.RecordObject(creator, "Add Segment");
        Vector3 direction = CalculatePathDirection(creator.focusIndex);
        Vector3 origin = path[3 * creator.focusIndex];
        path.AddSegment(creator.focusIndex + 1, origin + 1 * direction, origin + 2 * direction, origin + 3 * direction);
        creator.focusIndex += 1;
    }

    private void ActionRemoveAnchor()
    {
        Undo.RecordObject(creator, "Remove Segment");
        path.RemoveAnchor(creator.focusIndex);
        creator.focusIndex = Mathf.Clamp(creator.focusIndex, 0, path.NumAnchors - 1);
    }

    bool MouseRaycast(Event guiEvent, out RaycastHit raycastHit)
    {
        return Physics.Raycast(HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition), out raycastHit);
    }

    private Vector3 CalculatePathDirection(int anc)
    {
        if (0 <= anc && anc <= path.NumAnchors - 1)
        {
            if (3 * anc + 1 <= path.NumPoints - 1)
                return (path[3 * creator.focusIndex + 1] - path[3 * creator.focusIndex]).normalized;
            else
                return (path[3 * creator.focusIndex] - path[3 * creator.focusIndex - 1]).normalized;
        }
        else
        {
            throw new System.IndexOutOfRangeException("Anchor index out of range");
        }
    }

    private void InteractableHandles()
    {
        //creator.transform.TransformPoint();
        //creator.transform.InverseTransformPoint();
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
                if (Handles.Button(creator.transform.TransformPoint(path.GetAnchor(i)), Quaternion.identity, 0.2f, 0.2f, Handles.SphereHandleCap)) //Click SphereHandleCap to focus on clicked anchor
                {
                    creator.focusIndex = i;
                }
            }
        }
    }

    private void TranslateAnchorHandles()
    {
        
        int indexCurrent = creator.focusIndex * 3;
        Vector3 anchorStoredPos = path[indexCurrent];
        Vector3 anchorHandle = Handles.PositionHandle(creator.transform.TransformPoint(anchorStoredPos), Quaternion.identity);
        Vector3 anchorHandlePos = creator.transform.InverseTransformPoint(anchorHandle);
        if (newRaycastHit)
            anchorHandlePos = lastMouseRaycastHit.point;
        if (anchorStoredPos != anchorHandlePos)
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
                path.MovePoint(indexCurrent + 1, anchorStoredPos + Vector3.Distance(anchorStoredPos, path[indexCurrent + 1]) * (anchorHandleRot * Vector3.forward));
            if (0 <= indexCurrent - 1 && indexCurrent - 1 <= path.NumPoints - 1)
                path.MovePoint(indexCurrent - 1, anchorStoredPos - Vector3.Distance(anchorStoredPos, path[indexCurrent - 1]) * (anchorHandleRot * Vector3.forward));
        }
        //Moving strength handle ahead constrained by rotation
        if (0 <= indexCurrent + 1 && indexCurrent + 1 <= path.NumPoints - 1)
        {
            Vector3 strengthHandle = Handles.FreeMoveHandle(creator.transform.TransformPoint(path[indexCurrent + 1]), Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereHandleCap);
            Vector3 strengthHandlePos = creator.transform.InverseTransformPoint(strengthHandle);
            if (path[indexCurrent + 1] != strengthHandlePos)
            {
                Undo.RecordObject(creator, "Adjust Path Strength");
                path.MovePoint(indexCurrent + 1, anchorStoredPos + Vector3.Distance(anchorStoredPos, strengthHandlePos) * (anchorHandleRot * Vector3.forward));
            }
        }
        //Moving strength handle behind constrained by rotation
        if (0 <= indexCurrent - 1 && indexCurrent - 1 <= path.NumPoints - 1)
        {
            Vector3 strengthHandle = Handles.FreeMoveHandle(creator.transform.TransformPoint(path[indexCurrent - 1]), Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereHandleCap);
            Vector3 strengthHandlePos = creator.transform.InverseTransformPoint(strengthHandle);
            if (path[indexCurrent - 1] != strengthHandlePos)
            {
                Undo.RecordObject(creator, "Adjust Path Strength");
                path.MovePoint(indexCurrent - 1, anchorStoredPos - Vector3.Distance(anchorStoredPos, strengthHandlePos) * (anchorHandleRot * Vector3.forward));
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
        Handles.SphereHandleCap(0, creator.transform.TransformPoint(path.GetAnchor(creator.focusIndex)), Quaternion.identity, 0.3f, EventType.Repaint);
    }

    private bool KeyDown(Event guiEvent, KeyCode key)
    {
        return (guiEvent.keyCode == key && guiEvent.type == EventType.KeyDown);
    }
}
