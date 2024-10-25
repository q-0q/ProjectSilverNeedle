using System;
using Cinemachine;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Input;

public class DramaticVirtualCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
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