using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;

public class CameraEffects : MonoBehaviour
{
    private Rigidbody playerRB;
    private Camera cam;

    private VisualEffect speedLines;
    [SerializeField] private Volume speedVolume;
    private readonly float speedEffectsStart = 15;
    private Average playerSpeed;

    [SerializeField] private CameraEffect<float> fov;
    [SerializeField] private CameraEffect<float> tilt;
    [SerializeField] private CameraEffect<float> postProcessing;

    [System.Serializable]
    public struct CameraEffect<T>
    {
        [HideInInspector] public T baseValue;
        [HideInInspector] public T value;
        [HideInInspector] public float lerpValue;

        public T maxValue;
    }

    private void Awake()
    {
        playerRB = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<Rigidbody>();
        playerSpeed = new Average(300);

        cam = Camera.main;

        fov.baseValue = cam.fieldOfView;
        fov.value = fov.baseValue;

        postProcessing.baseValue = 0;
        postProcessing.value = postProcessing.baseValue;

        tilt.baseValue = cam.transform.rotation.eulerAngles.z;
        tilt.value = tilt.baseValue;

        speedLines = Camera.main.GetComponent<VisualEffect>();
        speedLines.Stop();
    }

    private void Update()
    {
        playerSpeed.Update(playerRB.velocity.magnitude);
        if (playerRB.velocity.magnitude <= 1)
        {
            playerSpeed.Reset();
        }

        fov.lerpValue = Mathf.Clamp((playerSpeed.GetAverage() - speedEffectsStart) / speedEffectsStart, 0, 1);
        fov.value = Mathf.Lerp(fov.baseValue, fov.maxValue, fov.lerpValue);

        //cam.transform.rotation = Quaternion.Euler(0, 0, tilt.value);

        postProcessing.lerpValue = Mathf.Clamp((playerSpeed.GetAverage() - speedEffectsStart) / speedEffectsStart, 0, 1);
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

        tilt.lerpValue = 0;
        while (tilt.lerpValue <= 1)
        {
            tilt.lerpValue += 0.01f;

            tilt.value = Mathf.Lerp(tilt.value, startingValue, endingValue);

            //cam.transform.rotation = Quaternion.Euler(0, 0, tilt.value);
            yield return null;
        }
    }
}