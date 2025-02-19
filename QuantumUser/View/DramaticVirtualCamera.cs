using System;
using Cinemachine;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Input = UnityEngine.Input;

public class DramaticVirtualCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private Volume _volume;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _volume = FindObjectOfType<Volume>();
    }

    private void Update()
    {
        if (CameraTargetController.Instance.Player0Dramatic || CameraTargetController.Instance.Player1Dramatic)
        {
            _virtualCamera.Priority = 20;
        }
        else
        {
            _virtualCamera.Priority = 0;
        }
    }
}