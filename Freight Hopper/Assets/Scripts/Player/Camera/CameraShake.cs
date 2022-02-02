using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool camShakeAcive = true; //on or off

    [System.Serializable]
    public struct TraumaSettings
    {
        public float mult; //the power of the shake
        public float mag; //the range of movment
        public float rotMag; //the rotational power
        public float depthMag; //the depth multiplier
        public float decayTime; //how quickly the shake falls off

        public TraumaSettings(float mult = 16, float mag = 0.8f, float rotMag = 17f, float depthMag = 0.6f, float decayTime = 1.3f)
        {
            this.mult = mult;
            this.mag = mag;
            this.rotMag = rotMag;
            this.depthMag = depthMag;
            this.decayTime = decayTime;
        }
    }

    //Get a perlin float between -1 & 1, based off the time counter.
    private float GetFloat(float seed, float timeCounter)
    {
        return (Mathf.PerlinNoise(seed, timeCounter) - 0.5f) * 2f;
    }

    //use the above function to generate a Vector3, different seeds are used to ensure different numbers
    private Vector3 GetVec3(float traumaDepthMag, float timeCounter)
    {
        return new Vector3(
            GetFloat(1, timeCounter),
            GetFloat(10, timeCounter),
            //deapth modifier applied here
            GetFloat(100, timeCounter) * traumaDepthMag
            );
    }

    public void StartCameraShake(float truama, TraumaSettings traumaSettings)
    {
        if (!camShakeAcive)
        {
            return;
        }
        truama = Mathf.Clamp01(truama);
        StartCoroutine(Shake(truama, traumaSettings));
    }

    private IEnumerator Shake(float truama, TraumaSettings traumaSettings)
    {
        float timeCounter = 0; //counter stored for smooth transition
        while (truama > 0)
        {
            //increase the time counter (how fast the position changes) based off the traumaMult and some root of the Trauma
            timeCounter += Time.deltaTime * Mathf.Pow(truama, 0.3f) * traumaSettings.mult;
            //Bind the movement to the desired range
            Vector3 newPos = GetVec3(traumaSettings.depthMag, timeCounter) * traumaSettings.mag * truama; ;
            this.transform.localPosition += newPos;
            //rotation modifier applied here
            this.transform.localRotation *= Quaternion.Euler(newPos * traumaSettings.rotMag);
            //decay faster at higher values
            truama -= Time.deltaTime * traumaSettings.decayTime * (truama + 0.3f);

            yield return null;
        }
    }

    public void StartCameraSway(float duration, Vector3 direction, float magnitude = 1)
    {
        StartCoroutine(CameraSway(duration, direction, magnitude));
    }

    private IEnumerator CameraSway(float duration, Vector3 direction, float magnitude = 1)
    {
        float totalDuration = duration;
        float t = 0;
        while (duration > 0)
        {
            t = -Mathf.Cos(duration * 2f * Mathf.PI / totalDuration) / 2f + 1f / 2f;
            this.transform.localPosition += Vector3.Lerp(Vector3.zero, direction * magnitude, t);
            duration -= Time.deltaTime;
            yield return null;
        }
    }
}