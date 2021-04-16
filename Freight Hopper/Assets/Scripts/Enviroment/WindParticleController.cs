using System.Collections;
using System.Collections.Generic;
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

    public void EnableParticles()
    {
        windEnabled = true;
    }

    public void DisableParticles()
    {
        windEnabled = false;
        if (particleSys != null)
        {
            particleSys.Stop();
        }
    }

    // These are all cached so that we are not finding them a dozens of times a second

    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.ShapeModule shapeModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule;

    public void SpawnParticleSystem(Vector3 size, Vector3 direction, Transform parent)
    {
        if (particles == null)
        {
            sourceTransform = parent;
            particles = Instantiate(Resources.Load("Particles/Wind Particles"), sourceTransform.position, sourceTransform.rotation, sourceTransform) as GameObject;
            particleSys = particles.GetComponent<ParticleSystem>();
            if (particles == null)
            {
                Debug.LogWarning("Particle system not found for " + name);
            }

            mainModule = particleSys.main;
            shapeModule = particleSys.shape;
            emissionModule = particleSys.emission;
            velocityOverLifetimeModule = particleSys.velocityOverLifetime;

            mainModule.startLifetime = size.z * 0.05f;
            shapeModule.scale = size;
            emissionModule.rateOverTime = size.x * size.y * 3;
            EnableParticles();
        }

        particleSys.Play();

        velocityOverLifetimeModule.x = direction.x;
        velocityOverLifetimeModule.y = direction.y;
        velocityOverLifetimeModule.z = direction.z;
    }

    private void FixedUpdate()
    {
        if (windEnabled && !particles)
        {
            particleSys.Play();
        }
    }
}