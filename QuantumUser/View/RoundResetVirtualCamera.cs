using System;
using Cinemachine;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Input;

public class RoundResetVirtualCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        GameFSMScreenTransition.OnCountdownStart += OnCountdown;
        GameFSMScreenTransition.OnResetStart += OnReset;
    }

    private void OnDisable()
    {
        GameFSMScreenTransition.OnCountdownStart -= OnCountdown;
        GameFSMScreenTransition.OnResetStart -= OnReset;
    }

    private void OnCountdown()
    {
        _virtualCamera.Priority = -100;
    }
    
    private void OnReset()
    {
        _virtualCamera.Priority = 100;
    }
}