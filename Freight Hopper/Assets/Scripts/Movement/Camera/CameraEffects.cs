using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;

public class CameraEffects : MonoBehaviour
{
    private Rigidbody playerRB;
    private MovementBehavior playerMovement;
    private Camera cam;

    private VisualEffect speedLines;
    private Volume speedVolume;
    private float speedEffectsStart;
    private Average playerSpeed;

    [SerializeField] private CameraEffect fov;
    [SerializeField] private CameraEffect tilt;
    [SerializeField] private CameraEffect postProcessing;

    [System.Serializable]
    private struct CameraEffect
    {
        [HideInInspector] public float baseValue;
        [HideInInspector] public float value;
        [HideInInspector] public float lerpValue;

        public float maxValue;
    }

    private void Awake()
    {
        playerRB = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<Rigidbody>();
        playerMovement = playerRB.gameObject.GetComponent<MovementBehavior>();
        speedEffectsStart = playerMovement.Speed * 3;
        playerSpeed = new Average(300);

        cam = GetComponent<Camera>();
        fov.baseValue = cam.fieldOfView;
        fov.value = fov.baseValue;

        speedVolume = Camera.main.GetComponentInChildren<Volume>();
        postProcessing.baseValue = 0;
        postProcessing.value = postProcessing.baseValue;

        tilt.baseValue = cam.transform.eulerAngles.z;
        tilt.value = tilt.baseValue;

        speedLines = Camera.main.GetComponent<VisualEffect>();
        speedLines.Stop();
    }

    private void Update()
    {
        playerSpeed.Update(playerRB.velocity.magnitude);
        if (playerRB.velocity.magnitude <= 1 && playerSpeed.CanReset())
        {
            playerSpeed.Reset();
            playerSpeed.ToggleReset();
        }
        else if (playerRB.velocity.magnitude >= 1 && !playerSpeed.CanReset())
        {
            playerSpeed.ToggleReset();
        }

        fov.lerpValue = Mathf.Clamp((playerSpeed.GetAverage() - playerMovement.Speed) / speedEffectsStart, 0, 1);
        fov.value = Mathf.Lerp(fov.baseValue, fov.maxValue, fov.lerpValue);

        postProcessing.lerpValue = Mathf.Clamp((playerSpeed.GetAverage() - playerMovement.Speed) / speedEffectsStart, 0, 1);
        postProcessing.value = Mathf.Lerp(postProcessing.baseValue, postProcessing.maxValue, postProcessing.lerpValue);

        cam.fieldOfView = fov.value;
        speedVolume.weight = postProcessing.value;

        if (playerSpeed.GetAverage() > speedEffectsStart)
        {
            speedLines.Play();
        }
        else
        {
            speedLines.Stop();
        }
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
            tilt.lerpValue += 0.01f;

            tilt.value = Mathf.Lerp(tilt.value, amount, tilt.lerpValue);

            if (startingValue < endingValue)
            {
                tilt.value = Mathf.Clamp(tilt.value, startingValue, endingValue);
            }
            else
            {
                tilt.value = Mathf.Clamp(tilt.value, endingValue, startingValue);
            }

            cam.transform.rotation = Quaternion.Euler(0, 0, tilt.value);
            yield return null;
        }
    }
}