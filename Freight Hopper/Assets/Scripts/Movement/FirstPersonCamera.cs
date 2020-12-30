using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FirstPersonCamera : MonoBehaviour
{
    private Rigidbody playerRB;
    private MovementBehavior playerMovement;
    private CinemachineVirtualCamera cam;

    [SerializeField] private CameraEffect fov;
    [SerializeField] private CameraEffect tilt;

    [System.Serializable]
    private struct CameraEffect
    {
        [HideInInspector] public float baseValue;
        [HideInInspector] public float value;
        [HideInInspector] public float addedValue;

        public float maxValue;
        public float transitionSmoothness;
    }

    private void Awake()
    {
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerMovement = playerRB.gameObject.GetComponent<MovementBehavior>();
        cam = GetComponent<CinemachineVirtualCamera>();
        fov.baseValue = cam.m_Lens.FieldOfView;
        fov.value = fov.baseValue;

        tilt.baseValue = cam.m_Lens.Dutch;
        tilt.value = tilt.baseValue;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        fov.addedValue = playerRB.velocity.magnitude - playerMovement.Speed;
        fov.value = Mathf.Lerp(fov.value, fov.baseValue + fov.addedValue, fov.transitionSmoothness);
        fov.value = Mathf.Clamp(fov.value, fov.baseValue, fov.maxValue);

        cam.m_Lens.FieldOfView = fov.value;
    }

    /// <summary>
    /// Expecting a value of -1 for left, 0 for none, 1 for right
    /// </summary>
    /// <param name="direction"></param>
    public void Tilt(int direction)
    {
        StartCoroutine(Tilting(direction));
    }

    private IEnumerator Tilting(int direction)
    {
        float tiltSign = Mathf.Sign(tilt.value);
        while (tilt.value != tilt.maxValue * direction)
        {
            if (direction != 0)
            {
                tilt.addedValue += 0.03f * direction;
            }
            else
            {
                tilt.addedValue += 0.03f * -tiltSign;
            }

            tilt.value = Mathf.Lerp(tilt.value, tilt.baseValue + tilt.addedValue, tilt.transitionSmoothness);

            if (direction != 0)
            {
                tilt.value = Mathf.Clamp(tilt.value, -tilt.maxValue, tilt.maxValue);
            }
            else
            {
                if (tiltSign == 1)
                {
                    tilt.value = Mathf.Clamp(tilt.value, 0, tilt.maxValue);
                }
                else
                {
                    tilt.value = Mathf.Clamp(tilt.value, -tilt.maxValue, 0);
                }
            }

            cam.m_Lens.Dutch = tilt.value;
            yield return null;
        }
    }
}