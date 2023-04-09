using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class CameraEffects : MonoBehaviour
{
    private Rigidbody playerRB;
    [SerializeField] private CinemachineVirtualCamera cam;

    private VisualEffect speedLines;

    [SerializeField] private float speedEffectsStart = 50;
    [SerializeField] private float speedEffectsEnd = 120;

    SoundManager playerSounds;
    [SerializeField] private CameraEffect<float> fov;
    [SerializeField] private Camera distanceCamera;

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
        playerSounds = Player.Instance.modules.soundManager;
        cam.m_Lens.FieldOfView = Settings.GetFOV;
        distanceCamera.fieldOfView = Settings.GetFOV;
        fov.baseValue = cam.m_Lens.FieldOfView;
        fov.value = fov.baseValue;
        fov.maxValue += fov.baseValue;

        speedLines = Camera.main.GetComponent<VisualEffect>();
        
        speedLines.Stop();
    }

    private bool goingSlowAgain;

    private void Update()
    {
        //Vector3 speedLineDirection = playerRB.transform.InverseTransformDirection(playerRB.velocity.normalized);
        speedLines.SetVector3("Direction", playerRB.velocity.normalized);
        float speed = playerRB.velocity.magnitude;

        fov.lerpValue = Mathf.Clamp((speed - speedEffectsStart) / speedEffectsEnd, 0, 1);
        fov.value = Mathf.Lerp(fov.baseValue, fov.maxValue, fov.lerpValue);

        speedLines.SetFloat("Scalar", fov.lerpValue + 1);

        cam.m_Lens.FieldOfView = fov.value;
        distanceCamera.fieldOfView = fov.value;

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
}