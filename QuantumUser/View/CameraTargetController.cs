using System;
using Cinemachine;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Input;

public class CameraTargetController : MonoBehaviour
{
    public static CameraTargetController Instance;

    private static float _cameraXPan = PlayerFSM.WallHalfLength.AsFloat - 7;
    private Vector3 _player0Pos;
    private Vector3 _player1Pos;

    public bool Player0Dramatic = false;
    public bool Player1Dramatic = false;
    
    private float _baseYPos;
    private float _yPulldown = 4.5f;
    private float groundBounceTimer;

    private void Awake()
    {
        Instance = this;
        _baseYPos = transform.position.y;
        
        QuantumEvent.Subscribe(listener: this, handler: (EventResetUnityView e) => ResetView());

    }

    private void ResetView()
    {
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
        // _player0Pos = Vector3.zero;
        // _player1Pos = Vector3.zero;

    }

    public void UpdatePlayerPos(Vector3 pos, int playerId, int dramaticRemaining, bool groundBounce)
    {
        if (playerId == 0)
        {
            _player0Pos = pos;
            Player0Dramatic = dramaticRemaining > 0;
            if (groundBounce) groundBounceTimer = 0;
        }
        else
        {
            _player1Pos = pos;
            Player1Dramatic = dramaticRemaining > 0;
            if (groundBounce) groundBounceTimer = 0;
        }
    }

    private float GetGroundBouncePulldown()
    {
        var groundBouncePulldown = Mathf.Clamp(Mathf.InverseLerp(0.75f, 0.25f, groundBounceTimer), 0, 1) * 2;
        Debug.Log(groundBouncePulldown);
        return groundBouncePulldown;
    }

    private void Update()
    {
        groundBounceTimer += Time.deltaTime;
        float x = (_player0Pos.x + _player1Pos.x) / 2;
        x = Mathf.Clamp(x, -_cameraXPan, _cameraXPan);
        var yPulldown = _yPulldown + GetGroundBouncePulldown();
        float y = Mathf.Clamp((Mathf.Max(_player0Pos.y - yPulldown, _player1Pos.y - yPulldown)), 0f, 1000f);
        transform.position =
            new Vector3(Mathf.Lerp(transform.position.x, x, Time.deltaTime * 6f), Mathf.Lerp(transform.position.y, y + _baseYPos, Time.deltaTime * 20f), transform.position.z);
    }
    
}