using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

public class HitParticles : QuantumEntityViewComponent
{
    private ParticleSystem _particleSystem;

    private float _emissionRate;
    // Start is called before the first frame update
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _emissionRate = _particleSystem.emission.rateOverTimeMultiplier;
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
    // void Update()
    // {
    //     // .emission.rateOverTimeMultiplier = HitstopSystem.IsHitstopActive(PredictedFrame) ? 0 : 1;
    //     var main = _particleSystem.main;
    //     main.simulationSpeed = HitstopSystem.IsHitstopActive(PredictedFrame) ? 0 : 1;
    // }
}
