using UnityEngine;

public class ParticleAttractor : MonoBehaviour
{
    public float pullStrength = 10f;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ParticleSystem particleSystem))
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
            int numParticlesAlive = particleSystem.GetParticles(particles);

            for (int i = 0; i < numParticlesAlive; i++)
            {
                Vector3 directionToAttractor = (transform.position - particles[i].position).normalized;
                particles[i].velocity += directionToAttractor * pullStrength * Time.deltaTime;
            }

            particleSystem.SetParticles(particles, numParticlesAlive);
        }
    }
}
