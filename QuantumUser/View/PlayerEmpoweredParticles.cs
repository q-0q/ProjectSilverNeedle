using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Client.StructWrapping;
using Photon.Deterministic;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LayerMask = UnityEngine.LayerMask;

public class PlayerEmpoweredParticles : QuantumEntityViewComponent
{
    private ParticleSystem _particleSystem;

    
    public override void OnInitialize()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
    
    public override void OnUpdateView()
    {
        if (!PredictedFrame.Has<HealthData>(EntityRef)) return;
        if (FsmLoader.FSMs[EntityRef] is not PlayerFSM playerFsm) return;

        if (playerFsm.IsBuffActive(PredictedFrame))
        {
            Debug.Log("PLAY");
            _particleSystem.Play(true);
            
        }
        else
        {
            Debug.Log("STOP");
            _particleSystem.Stop(true);
            
        }
    }
}