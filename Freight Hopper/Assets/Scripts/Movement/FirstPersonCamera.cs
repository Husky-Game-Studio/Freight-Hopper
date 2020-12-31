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
        Tilt(-10);
        Invoke(nameof(ResetTilt), 5);
    }

    private void FixedUpdate()
    {
        fov.addedValue = playerRB.velocity.magnitude - 2 * playerMovement.Speed;
        fov.value = Mathf.Lerp(fov.value, fov.baseValue + fov.addedValue, fov.transitionSmoothness);
        fov.value = Mathf.Clamp(fov.value, fov.baseValue, fov.maxValue);
        cam.m_Lens.FieldOfView = fov.value;
    }

    /// <summary>
    /// Tilts the player the given amount of degrees
    /// </summary>
    /// <param name="amount"></param>
    public void Tilt(float amount)
    {
        StartCoroutine(Tilting(amount));
    }

    /// <summary>
    /// Resets the tilt of the player back to the base value
    /// </summary>
    public void ResetTilt()
    {
        StartCoroutine(Tilting(tilt.baseValue - tilt.value));
    }

    private IEnumerator Tilting(float amount)
    {
        float startingValue = tilt.value;
        float endingValue = tilt.value + amount;

        while (tilt.value != endingValue)
        {
            tilt.addedValue += 0.02f * amount;

            tilt.value = Mathf.Lerp(tilt.value, tilt.value + tilt.addedValue, tilt.transitionSmoothness);

            if (startingValue < endingValue)
            {
                tilt.value = Mathf.Clamp(tilt.value, startingValue, endingValue);
            }
            else
            {
                tilt.value = Mathf.Clamp(tilt.value, endingValue, startingValue);
            }

            cam.m_Lens.Dutch = tilt.value;
            yield return null;
        }
    }
}