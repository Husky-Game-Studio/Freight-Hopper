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
    [SerializeField] private float speedEffectsStart = 50;
    [SerializeField] private float speedEffectsEnd = 120;

    [SerializeField] private SoundManager playerSounds;
    [SerializeField] private CameraEffect<float> fov;

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
        playerRB = Player.Instance.modules.rigidbody;

        cam = Camera.main;

        fov.baseValue = cam.fieldOfView;
        fov.value = fov.baseValue;
        fov.maxValue += fov.baseValue;

        speedLines = Camera.main.GetComponent<VisualEffect>();

        speedLines.Stop();
    }

    private bool goingSlowAgain = false;

    private void Update()
    {
        //Vector3 speedLineDirection = playerRB.transform.InverseTransformDirection(playerRB.velocity.normalized);
        speedLines.SetVector3("Direction", playerRB.velocity.normalized);
        float speed = playerRB.velocity.magnitude;

        fov.lerpValue = Mathf.Clamp((speed - speedEffectsStart) / speedEffectsEnd, 0, 1);
        fov.value = Mathf.Lerp(fov.baseValue, fov.maxValue, fov.lerpValue);

        speedLines.SetFloat("Scalar", fov.lerpValue + 1);

        cam.fieldOfView = fov.value;

        if (speed > speedEffectsStart)
        {
            speedLines.Play();
            playerSounds.Play("GoingFast");
            goingSlowAgain = true;
        }
        else
        {
            speedLines.Stop();
        }
        if (goingSlowAgain && speed < speedEffectsStart)
        {
            playerSounds.Stop("GoingFast");
            goingSlowAgain = false;
        }
    }

    private Color BlendGradients(Gradient a, Gradient b, float bias, float t)
    {
        Color fromA = a.Evaluate(t);
        Color fromB = b.Evaluate(t);

        Color final = Color.Lerp(fromA, fromB, bias);
        return final;
    }
}