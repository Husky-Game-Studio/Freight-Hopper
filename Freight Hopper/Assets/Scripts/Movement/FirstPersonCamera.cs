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

    private CameraEffect speedEffectsWeight;

    [System.Serializable]
    private struct CameraEffect
    {
        public float baseValue;
        public float value;
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

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        float addedFov = playerRB.velocity.magnitude - playerMovement.Speed;
        fov.value = Mathf.Lerp(fov.value, fov.baseValue + addedFov, fov.transitionSmoothness);
        fov.value = Mathf.Clamp(fov.value, fov.baseValue, fov.maxValue);

        cam.m_Lens.FieldOfView = fov.value;
    }
}