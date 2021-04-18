using UnityEngine;

public class WindParticleController : MonoBehaviour
{
    private bool windEnabled = false;
    private GameObject particles;
    private ParticleSystem particleSys;
    private Transform sourceTransform;

    private void Awake()
    {
        sourceTransform = this.transform;
    }

    /// <summary>
    /// Sets wind enabled to true, enabling the particles
    /// </summary>
    public void EnableParticles()
    {
        windEnabled = true;
    }

    /// <summary>
    /// Sets wind enabled to false, disabling the particles
    /// </summary>
    public void DisableParticles()
    {
        windEnabled = false;
    }

    // These are all cached so that we are not finding them a dozens of times a second

    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.ShapeModule shapeModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule;
    private ParticleSystem.TrailModule trailModule;

    /// <summary>
    /// Can be called multiple times, first call instantiates particle system with the given settings.
    /// Should be called multiple times if the particle system needs to be updated.
    /// </summary>
    public void SpawnParticleSystem(Vector3 offset, Vector3 size, Vector3 direction, Transform parent)
    {
        if (particles == null)
        {
            sourceTransform = parent;
            particles = Instantiate(Resources.Load("Particles/Wind Particles"), sourceTransform.position + offset, sourceTransform.rotation, sourceTransform) as GameObject;
            particleSys = particles.GetComponent<ParticleSystem>();
            if (particles == null)
            {
                Debug.LogWarning("Particle system not found for " + name);
            }

            mainModule = particleSys.main;
            shapeModule = particleSys.shape;
            emissionModule = particleSys.emission;
            velocityOverLifetimeModule = particleSys.velocityOverLifetime;
            trailModule = particleSys.trails;

            EnableParticles();
        }

        UpdateSettings(size, direction);

        particleSys.Play();
    }

    /// <summary>
    /// size and direction can be modified at runtime and therefore particle system needs to be updated
    /// </summary>
    public void UpdateSettings(Vector3 size, Vector3 direction)
    {
        velocityOverLifetimeModule.x = direction.x;
        velocityOverLifetimeModule.y = direction.y;
        velocityOverLifetimeModule.z = direction.z;

        mainModule.startLifetime = size.z * 0.05f;
        shapeModule.scale = size;
        emissionModule.rateOverTime = size.x * size.y * 5;
        trailModule.lifetime = 1 / size.z;
    }

    private void FixedUpdate()
    {
        if (windEnabled && particleSys != null)
        {
            particleSys.Play();
        }
        if (!windEnabled && particleSys != null)
        {
            particleSys.Stop();
        }
        DisableParticles();
    }
}