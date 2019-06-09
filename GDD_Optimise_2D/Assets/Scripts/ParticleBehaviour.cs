using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehaviour : MonoBehaviour
{
    private ParticleSystem particles;       // Reference to the Particle System.

    void Start()
    {
        // Finds the particle system component in this Object
        particles = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        // Checks if the particle system is playing, if it isn't, then destory this Particle System
        if (!particles.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
