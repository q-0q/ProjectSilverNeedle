using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Deterministic;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraView
    : MonoBehaviour
{
    private Transform _camera;
    
    public void Awake()
    {
        _camera = transform.GetChild(0);
        QuantumEvent.Subscribe(listener: this, handler: (EventEntityVibrate e) => CameraVibrate(e.entityRef, e.strength, e.duration, e.vibrato));
    }
    
    private void CameraVibrate(EntityRef entityRef, FP strength, FP duration, int vibrato)
    {
        _camera.localPosition = Vector3.zero;
        _camera.DOShakePosition(duration.AsFloat * 0.2f, strength.AsFloat * 0.5f, 30, 90f, false, true, ShakeRandomnessMode.Full);
    }
}
