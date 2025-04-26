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
        
        if (GameFsmLoader.LoadGameFSM(PredictedFrame).Fsm.IsInState(GameFSM.State.Loading)) return;
        if (GameFsmLoader.LoadGameFSM(PredictedFrame).Fsm.IsInState(GameFSM.State.Waiting)) return; 
        
        // Vector3 target = (transform.position - _camera.transform.position) + transform.position;
        // transform.LookAt(target);

        string characterName = fsm.Name;
        int frame = PredictedFrame.Get<AnimationData>(EntityRef).frame + 1;
        var fighterAnimation = fsm.StateMapConfig.FighterAnimation;
        if (fighterAnimation is null) return;

        var fighterAnimation1 = fighterAnimation.Get(fsm, null);

        if (fighterAnimation1 is not null)
        {
            string path = fighterAnimation1.Path;
            string fullPath = "Sprites/Characters/" + characterName + "/FrameGroups/" + path + "/" + path + "_" + frame;
            Sprite sprite = Resources.Load<Sprite>(fullPath);
            _renderer.sprite = sprite;
            _shadowCasterRenderer.sprite = sprite;
            var flip = !PredictedFrame.Get<PlayerDirection>(EntityRef).FacingRight;
            _renderer.flipX = flip;
            _shadowCasterRenderer.flipX = flip;
        }

        
        // offense / defense sorting
        if (fsm is null) return;
        bool back = fsm.Fsm.IsInState(PlayerFSM.PlayerState.Block) || fsm.Fsm.IsInState(PlayerFSM.PlayerState.Hit) || fsm.Fsm.IsInState(PlayerFSM.PlayerState.CutsceneReactor);
        if (fsm.sendToBack)
        {
            gameObject.layer = LayerMask.NameToLayer("AEBack");
        }
        else
        {
            gameObject.layer = (back) ? LayerMask.NameToLayer("PlayerBack") : LayerMask.NameToLayer("PlayerFront");
        }
        


        try
        {
            Vector3 pos = PredictedFrame.Get<Transform3D>(EntityRef).Position.ToUnityVector3();
            CameraTargetController.Instance.UpdatePlayerPos(pos, PredictedFrame.Get<PlayerLink>(EntityRef).Player,
                PredictedFrame.Get<DramaticData>(EntityRef).remaining, PredictedFrame.Get<DramaticData>(EntityRef).darkRemaining, PredictedFrame.Get<TrajectoryData>(EntityRef).groundBounce);

            HealthBarController.Instance.UpdatePlayerHealth(PredictedFrame.Get<PlayerLink>(EntityRef).Player,
                PredictedFrame.Get<HealthData>(EntityRef).health.AsFloat,
                PredictedFrame.Get<ComboData>(EntityRef).length,
                PredictedFrame.Get<ScoreData>(EntityRef).score);
            
            MeterBarController.Instance.UpdatePlayerMeter(PredictedFrame.Get<PlayerLink>(EntityRef).Player,
                PredictedFrame.Get<HealthData>(EntityRef).meter.AsFloat);
        }
        catch
        {
            // Debug.LogError(e);
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
