using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Input;

public class UnityInput : MonoBehaviour
{
  
    private PlayerInput _playerInput;
  
    // for determining numpad direction
    private const float DeadzoneSize = 0.1f;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    }

    public void PollInput(CallbackPollInput callback)
    {
        Quantum.Input input = new Quantum.Input();

        // Note: Use GetButton not GetButtonDown/Up Quantum calculates up/down itself.
    
        int uN = GetUnflippedNumpadDirection();
        input.UnflippedNumpadDirection = uN;
        
        input.L =  _playerInput.actions["L"].IsPressed();
        input.M =  _playerInput.actions["M"].IsPressed();
        input.H =  _playerInput.actions["H"].IsPressed();
        input.S =  _playerInput.actions["S"].IsPressed();
        input.T =  _playerInput.actions["T"].IsPressed();
        input.Dash =  _playerInput.actions["Dash"].IsPressed();

        input.Jump = _playerInput.actions["Direction"].IsPressed() && (uN == 7 || uN == 8 || uN == 9);
        
        callback.SetInput(input, DeterministicInputFlags.Repeatable);
    }

  
    // 7 8 9
    // 4 5 6
    // 1 2 3
    private int GetUnflippedNumpadDirection()
    {
        Vector2 raw = _playerInput.actions["Direction"].ReadValue<Vector2>();

        float rawAngle = Vector2.SignedAngle(raw, Vector2.up);
        float sliceSize = 360f / 8f;
        int i = Mathf.FloorToInt((rawAngle + (sliceSize * 0.5f))/ sliceSize);

        int output = 5;
        if (raw.magnitude > DeadzoneSize)
        {
            switch (i)
            {
                case 0:
                {
                    output = 8;
                    break;
                }
                case 1:
                {
                    output = 9;
                    break;
                }
                case 2:
                {
                    output = 6;
                    break;
                }
                case 3:
                {
                    output = 3;
                    break;
                }
                case 4:
                {
                    output = 2;
                    break;
                }
                case -1:
                {
                    output = 7;
                    break;
                }
                case -2:
                {
                    output = 4;
                    break;
                }
                case -3:
                {
                    output = 1;
                    break;
                }
                case -4:
                {
                    return 2;
                }
            }
        }
    
        return output;
    }
}