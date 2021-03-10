using UnityEngine;
using UnityEditor;

public class EditorPoint : Editor
{
    private Vector3 position;
    private Quaternion rotation;
    public Vector3 Position => position;
    public Quaternion Rotation => rotation;

    private Operation operation = Operation.Translate;

    private enum Operation
    {
        Translate,
        Rotate
    }

    private KeyCode translateKey;
    private KeyCode rotateKey;

    public EditorPoint(Vector3 position, Quaternion rotation, KeyCode translateKey = KeyCode.W, KeyCode rotateKey = KeyCode.E)
    {
        this.position = position;
        this.rotation = rotation;
        this.translateKey = translateKey;
        this.rotateKey = rotateKey;
    }
}