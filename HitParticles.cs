using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

public class HitParticles : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    // Start is called before the first frame update
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        QuantumEvent.Subscribe(listener: this, handler: (EventPlayerHit e) => TriggerParticles(e.position, e.angle));

    }

    private void TriggerParticles(FPVector2 position, FP angle)
    {
        transform.position = new Vector3(position.X.AsFloat, position.Y.AsFloat, 0);
        transform.rotation = Quaternion.Euler(0, 0, -angle.AsFloat);
        _particleSystem.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
