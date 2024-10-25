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

public class PlayerSprite : QuantumEntityViewComponent
{
    private SpriteRenderer _renderer;
    private SpriteRenderer _shadowCasterRenderer;
    
    public override void OnInitialize()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _shadowCasterRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        
        QuantumEvent.Subscribe(listener: this, handler: (EventEntityVibrate e) => PlayerVibrate(e.entityRef, e.strength, e.duration, e.vibrato));

    }

    public override void OnUpdateView()
    {
        
        if (!PredictedFrame.Has<AnimationData>(EntityRef)) return;
        
        Characters.CharacterEnum characterEnum = (Characters.CharacterEnum)PredictedFrame.Get<PlayerLink>(EntityRef).characterId;
        string characterName = Characters.Get(characterEnum).Name;
        string path = "Sprites/Characters/" + characterName + "/Frames/" + characterName;
        int frame = PredictedFrame.Get<AnimationData>(EntityRef).frame + 1;
        Sprite sprite = Resources.Load<Sprite>(path + frame);

        
        // offense / defense sorting
        PlayerFSM fsm = Util.GetPlayerFSM(PredictedFrame, EntityRef);
        bool back = fsm.Fsm.IsInState(PlayerFSM.State.Block) || fsm.Fsm.IsInState(PlayerFSM.State.Hit);
        gameObject.layer = back ? LayerMask.NameToLayer("PlayerBack") : LayerMask.NameToLayer("PlayerFront");
        
        _renderer.sprite = sprite;
        _shadowCasterRenderer.sprite = sprite;
        var flip = !PredictedFrame.Get<PlayerDirection>(EntityRef).FacingRight;
        _renderer.flipX = flip;
        _shadowCasterRenderer.flipX = flip;

        Vector3 pos = PredictedFrame.Get<Transform3D>(EntityRef).Position.ToUnityVector3();
        CameraTargetController.Instance.UpdatePlayerPos(pos, PredictedFrame.Get<PlayerLink>(EntityRef).Player, 
            PredictedFrame.Get<DramaticData>(EntityRef).remaining);
        
        HealthBarController.Instance.UpdatePlayerHealth(PredictedFrame.Get<PlayerLink>(EntityRef).Player,
            PredictedFrame.Get<HealthData>(EntityRef).health.AsFloat,
            PredictedFrame.Get<ComboData>(EntityRef).length,
            PredictedFrame.Get<ScoreData>(EntityRef).score);
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
