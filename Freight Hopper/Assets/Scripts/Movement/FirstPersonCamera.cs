using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FirstPersonCamera : MonoBehaviour
{
    private Rigidbody playerRB;
    private CinemachineVirtualCamera cam;
    [SerializeField] private CameraEffect fov;
    private CameraEffect speedEffectsWeight;

    [System.Serializable]
    private struct CameraEffect
    {
        public float baseValue;
        public float scalingConstant;
        public float maxValue;
    }

    private void Awake()
    {
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        cam = GetComponent<CinemachineVirtualCamera>();
        fov.baseValue = cam.m_Lens.FieldOfView;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        cam.m_Lens.FieldOfView = Mathf.Clamp((Mathf.Pow(playerRB.velocity.magnitude / 4 * fov.scalingConstant, 2)) * 60, fov.baseValue, fov.maxValue);
    }
}