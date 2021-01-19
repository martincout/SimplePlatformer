using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    public new ParticleSystem particleSystem;

    public void PlayParticle(Vector3 attackerPos)
    {
        Vector3 dir = transform.position - attackerPos;
        dir = transform.position + dir.normalized;
        ParticleSystem instance = Instantiate(particleSystem,dir,Quaternion.identity);
        Destroy(instance.gameObject, 1f);
    }

    public void PlayParticle()
    {
        ParticleSystem instance = Instantiate(particleSystem, transform.position, Quaternion.identity);
        Destroy(instance.gameObject, 1f);
    }
}
