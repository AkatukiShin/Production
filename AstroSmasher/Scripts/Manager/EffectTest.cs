using System;
using UnityEngine;

public class EffectTest : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems;

    void Start()
    {
       Invoke("PlayParticles", 5.0f);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) PlayParticles();
    }

    public void PlayParticles()
    {
        for (int i = 0; i < 4; i++)
        {
            particleSystems[i].Play();
        }
    }

    public void StopParticles()
    {
        foreach (var ps in particleSystems)
        {
            ps.Stop();
        }
    }

    public void SetParticlesSpeed(float speed)
    {
        foreach (var ps in particleSystems)
        {
            var main = ps.main;
            main.simulationSpeed = speed;
        }
    }
}