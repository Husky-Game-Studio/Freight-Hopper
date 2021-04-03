using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraBehavior : MonoBehaviour
{
    public Vector3 myPos;
    public Transform myPlay;
    Vector3 pointToAxis;
    Vector3 LastMousePosition;

    float Direction = 0.05f;

    //Got part of location tracking from: https://forum.unity.com/threads/making-camera-follow-an-object.32831/

    public void Update()
    {

        //Helps for Zoom In + Out 

        //Helps with LookAt ((found in AxisBehavior))
        pointToAxis = gameObject.transform.position - myPlay.position;
        gameObject.transform.forward = -1 * pointToAxis;


        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            gameObject.transform.position += Vector3.Normalize(-pointToAxis);
        }

        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            gameObject.transform.position += Vector3.Normalize(pointToAxis);
        }
        //Orbit
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButtonDown(0))
        {
            LastMousePosition = Input.mousePosition;
        }

        else if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(0))
        {
            CameraTumbleX();
            CameraTumbleY();
            LastMousePosition = Input.mousePosition;
        }

    }

    //From Week 6 example
    void CameraTumbleX()
    {
        Vector3 deltaMouse = Input.mousePosition - LastMousePosition;

        Quaternion q = Quaternion.AngleAxis(Direction * deltaMouse.x, Vector3.up);

        // 2. we need to rotate the camera position
        Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
        Matrix4x4 invP = Matrix4x4.TRS(-myPlay.localPosition, Quaternion.identity, Vector3.one);
        r = invP.inverse * r * invP;
        Vector3 newCameraPos = r.MultiplyPoint(gameObject.transform.localPosition);

        gameObject.transform.localPosition = newCameraPos;
        gameObject.transform.localRotation = q * gameObject.transform.localRotation;


    }

    void CameraTumbleY()
    {
        Vector3 deltaMouse = Input.mousePosition - LastMousePosition;

        Quaternion q = Quaternion.AngleAxis(Direction * deltaMouse.y, Vector3.right);

        // 2. we need to rotate the camera position
        Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
        Matrix4x4 invP = Matrix4x4.TRS(-myPlay.localPosition, Quaternion.identity, Vector3.one);
        r = invP.inverse * r * invP;
        Vector3 newCameraPos = r.MultiplyPoint(gameObject.transform.localPosition);

        gameObject.transform.localPosition = newCameraPos;
        gameObject.transform.localRotation = q * gameObject.transform.localRotation;

    }
}
