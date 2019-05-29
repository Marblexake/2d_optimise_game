using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehaviour : MonoBehaviour
{
    private ParticleSystem particles;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if (!particles.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
