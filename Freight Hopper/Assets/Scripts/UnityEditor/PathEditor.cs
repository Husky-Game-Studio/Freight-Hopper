using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    Rect windowRect = new Rect(20,40,10,10);
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
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent(" ", "[Numpad 7]"), GUILayout.Width(120.0f)))
        {
            
        }
        GUI.color = Color.red;
        if (GUILayout.Button(new GUIContent("Match Pos/Rot", "[Numpad 8]"), GUILayout.Width(120.0f)))
        {

        }
        GUI.color = defaultColor;
        if (GUILayout.Button(new GUIContent(" ", "[Numpad 9]"), GUILayout.Width(120.0f)))
        {

        }
        GUILayout.EndHorizontal();

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

        GUI.DragWindow();
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
        //Clickable Points
        for (int i = 0; i < path.NumAnchors; i++)
        {
            if (i != creator.focusIndex)
            {
                Handles.color = Color.cyan;
                if (Handles.Button(path.GetAnchor(i), Quaternion.identity, 0.2f, 0.2f, Handles.SphereHandleCap))
                {
                    creator.focusIndex = i;
                }
            }
        }

        //Translate Anchor
        if (operation == Operation.Translate)
        {
            int indexCurrent = creator.focusIndex * 3;
            Vector3 anchorStoredPos = path[indexCurrent];
            Vector3 anchorHandlePos = Handles.PositionHandle(anchorStoredPos, Quaternion.identity);
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

        //Rotate Anchor
        if (operation == Operation.Rotate)
        {
            Handles.color = Color.green;
            int indexCurrent = creator.focusIndex * 3;
            Vector3 anchorStoredPos = path[indexCurrent];
            Quaternion anchorStoredRot = Quaternion.LookRotation(CalculatePathDirection(creator.focusIndex));
            Quaternion anchorHandleRot = Handles.RotationHandle(anchorStoredRot, anchorStoredPos);
            if (newRaycastHit)
                anchorHandleRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(CalculatePathDirection(creator.focusIndex), lastMouseRaycastHit.normal));
            if (anchorStoredRot != anchorHandleRot)
            {
                Undo.RecordObject(creator, "Rotate Anchor Point");
                if (0 <= indexCurrent + 1 && indexCurrent + 1 <= path.NumPoints - 1)
                    path.MovePoint(indexCurrent + 1, anchorStoredPos + Vector3.Distance(anchorStoredPos, path[indexCurrent + 1]) * (anchorHandleRot * Vector3.forward));
                if (0 <= indexCurrent - 1 && indexCurrent - 1 <= path.NumPoints - 1)
                    path.MovePoint(indexCurrent - 1, anchorStoredPos - Vector3.Distance(anchorStoredPos, path[indexCurrent - 1]) * (anchorHandleRot * Vector3.forward));
            }
            if (0 <= indexCurrent + 1 && indexCurrent + 1 <= path.NumPoints - 1)
            {
                Vector3 strengthHandlePos1 = Handles.FreeMoveHandle(path[indexCurrent + 1], Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereHandleCap);
                if (path[indexCurrent + 1] != strengthHandlePos1)
                {
                    Undo.RecordObject(creator, "Adjust Path Strength");
                    path.MovePoint(indexCurrent + 1, anchorStoredPos + Vector3.Distance(anchorStoredPos, strengthHandlePos1) * (anchorHandleRot * Vector3.forward));
                }
            }
            if (0 <= indexCurrent - 1 && indexCurrent - 1 <= path.NumPoints - 1)
            {
                Vector3 strengthHandlePos2 = Handles.FreeMoveHandle(path[indexCurrent - 1], Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereHandleCap);
                if (path[indexCurrent - 1] != strengthHandlePos2)
                {
                    Undo.RecordObject(creator, "Adjust Path Strength");
                    path.MovePoint(indexCurrent - 1, anchorStoredPos - Vector3.Distance(anchorStoredPos, strengthHandlePos2) * (anchorHandleRot * Vector3.forward));
                }
            }
        }
    }

    private void Visuals()
    {
        //Bezier Lines
        for (int i = 0; i < path.NumSegments; i++)
        {
            Vector3[] points = path.GetSegment(i);
            Handles.color = Color.green;
            Handles.DrawLine(points[0], points[1]);
            Handles.DrawLine(points[2], points[3]);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.white, null, 2);
        }

        //Handle for anchor focused on
        Handles.color = Color.green;
        Handles.SphereHandleCap(0, path.GetAnchor(creator.focusIndex), Quaternion.identity, 0.3f, EventType.Repaint);
    }

    private bool KeyDown(Event guiEvent, KeyCode key)
    {
        return (guiEvent.keyCode == key && guiEvent.type == EventType.KeyDown);
    }
}
