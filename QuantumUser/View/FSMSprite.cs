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

public class FSMSprite : QuantumEntityViewComponent
{
    private SpriteRenderer _renderer;
    private SpriteRenderer _shadowCasterRenderer;
    private Color _color;
    private Camera _camera;
    public override void OnInitialize()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _shadowCasterRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _camera = Camera.main;
        
        QuantumEvent.Subscribe(listener: this, handler: (EventEntityVibrate e) => PlayerVibrate(e.entityRef, e.strength, e.duration, e.vibrato));

    }
    
    public override void OnUpdateView()
    {
        
        FSM fsm = FsmLoader.GetFsm(EntityRef);
        
        if (!PredictedFrame.Has<AnimationData>(EntityRef)) return;

        // Vector3 target = (transform.position - _camera.transform.position) + transform.position;
        // transform.LookAt(target);

        string characterName = fsm.Name;
        int frame = PredictedFrame.Get<AnimationData>(EntityRef).frame + 1;
        int path = PredictedFrame.Get<AnimationData>(EntityRef).path;
        var pathEnum = fsm.AnimationPathsEnum;
        string stringPath = Enum.ToObject(pathEnum, path).ToString();
        string fullPath = "Sprites/Characters/" + characterName + "/FrameGroups/" + stringPath + "/" + stringPath + "_" + frame;
        Sprite sprite = Resources.Load<Sprite>(fullPath);
        _renderer.sprite = sprite;

        
        // offense / defense sorting
        if (fsm is null) return;
        bool back = fsm.Fsm.IsInState(PlayerFSM.PlayerState.Block) || fsm.Fsm.IsInState(PlayerFSM.PlayerState.Hit) || fsm.Fsm.IsInState(PlayerFSM.PlayerState.CutsceneReactor);
        gameObject.layer = back ? LayerMask.NameToLayer("PlayerBack") : LayerMask.NameToLayer("PlayerFront");
        
        _shadowCasterRenderer.sprite = sprite;
        var flip = !PredictedFrame.Get<PlayerDirection>(EntityRef).FacingRight;
        _renderer.flipX = flip;
        _shadowCasterRenderer.flipX = flip;


        try
        {
            Vector3 pos = PredictedFrame.Get<Transform3D>(EntityRef).Position.ToUnityVector3();
            CameraTargetController.Instance.UpdatePlayerPos(pos, PredictedFrame.Get<PlayerLink>(EntityRef).Player,
                PredictedFrame.Get<DramaticData>(EntityRef).remaining);

            HealthBarController.Instance.UpdatePlayerHealth(PredictedFrame.Get<PlayerLink>(EntityRef).Player,
                PredictedFrame.Get<HealthData>(EntityRef).health.AsFloat,
                PredictedFrame.Get<ComboData>(EntityRef).length,
                PredictedFrame.Get<ScoreData>(EntityRef).score);
        }
        catch
        {
            // ignored
        }
    }

    private void Update()
    {
        _renderer.transform.localPosition =
            Vector3.Lerp(_renderer.transform.localPosition, Vector3.zero, Time.deltaTime);
    }

    private void PlayerVibrate(EntityRef entityRef, FP strength, FP duration, int vibrato)
    {
        if (entityRef != EntityRef) return;
        _renderer.transform.localPosition = Vector3.zero;
        _renderer.transform.DOShakePosition(duration.AsFloat * 0.5f, strength.AsFloat, vibrato, 90f, false, true, ShakeRandomnessMode.Full);
    }
}
