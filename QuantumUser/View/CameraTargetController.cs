using System;
using Cinemachine;
using Photon.Deterministic;
using Quantum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Input;

public class CameraTargetController : MonoBehaviour
{
    public static CameraTargetController Instance;


    public static float cameraMinCharDistance = 10;
    public static float cameraMaxCharDistance = 20;
    public static float cameraMaxZ = 4;
    public static float cameraMinZ = 1;
    public static float cameraMaxY = 0.5f;
    public static float cameraMinY = -0.25f;
    
    
    private static float _cameraXPan = PlayerFSM.WallHalfLength.AsFloat - 3;
    private Vector3 _player0Pos;
    private Vector3 _player1Pos;

    public bool Player0Dramatic = false;
    public bool Player1Dramatic = false;
    
    public bool Player0InAir = false;
    public bool Player1InAir = false;
    
    public bool Player0AirHit = false;
    public bool Player1AirHit = false;
    
    public bool Player0Dark = false;
    public bool Player1Dark = false;
    
    private float _baseYPos;
    private float _yPulldown = 3f;
    private float _hitBonusYPulldown = 2f;
    private float groundBounceTimer;
    
    
    public Frame Player0Frame;
    public Frame Player1Frame;

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

    public void UpdatePlayerPos(Vector3 pos, int playerId, int dramaticRemaining, int darkRemaining, bool groundBounce, bool airHit, Frame frame)
    {
        if (HitstopSystem.IsHitstopActive(frame)) return;
            
        if (playerId == 0)
        {
            _player0Pos = pos;
            Player0Dramatic = dramaticRemaining > 0;
            Player0Dark = darkRemaining > 0;
            Player0AirHit = airHit;
            if (groundBounce) groundBounceTimer = 0;
            Player0Frame = frame;
            Player0InAir = (FsmLoader.FSMs[Util.GetPlayer(frame, playerId)].Fsm.IsInState(PlayerFSM.PlayerState.Air));
        }
        else
        {
            _player1Pos = pos;
            Player1Dramatic = dramaticRemaining > 0;
            Player1Dark = darkRemaining > 0;
            Player1AirHit = airHit;
            if (groundBounce) groundBounceTimer = 0;
            Player1Frame = frame;
            Player1InAir = (FsmLoader.FSMs[Util.GetPlayer(frame, playerId)].Fsm.IsInState(PlayerFSM.PlayerState.Air));
        }
    }

    private float GetGroundBouncePulldown()
    {
        var groundBouncePulldown = Mathf.Clamp(Mathf.InverseLerp(0.75f, 0.25f, groundBounceTimer), 0, 1) * 2;
        // Debug.Log(groundBouncePulldown);
        return groundBouncePulldown;
    }

    private void Update()
    {
        
        groundBounceTimer += Time.deltaTime;
        float x = (_player0Pos.x + _player1Pos.x) / 2;
        x = Mathf.Clamp(x, -_cameraXPan, _cameraXPan);
        var yPulldown = _yPulldown + GetGroundBouncePulldown();
        
        if ((Player1AirHit && !Player0InAir) || (Player0AirHit && !Player1InAir)) yPulldown += _hitBonusYPulldown;
        
        
        float y = Mathf.Clamp((Mathf.Max(_player0Pos.y - yPulldown, _player1Pos.y - yPulldown)), 0f, 1000f);

        float deltaX = Mathf.Abs(_player0Pos.x - _player1Pos.x);
        var inverseLerp = Mathf.InverseLerp(cameraMinCharDistance, cameraMaxCharDistance, deltaX);
        float z = Mathf.Lerp(cameraMaxZ, cameraMinZ, inverseLerp);
        float yMod = Mathf.Lerp(cameraMinY, cameraMaxY, inverseLerp);
        
        transform.position =
            new Vector3(Mathf.Lerp(transform.position.x, x, Time.deltaTime * 12f), Mathf.Lerp(transform.position.y, y + _baseYPos + yMod, Time.deltaTime * 20f), z);
    }
    
}