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

public class SmearSprite : QuantumEntityViewComponent
{
    private SpriteRenderer _renderer;
    public override void OnInitialize()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
    
    public override void OnUpdateView()
    {
        
        FSM fsm = FsmLoader.GetFsm(EntityRef);
        
        if (GameFsmLoader.LoadGameFSM(PredictedFrame).Fsm.IsInState(GameFSM.State.Loading)) return;
        if (GameFsmLoader.LoadGameFSM(PredictedFrame).Fsm.IsInState(GameFSM.State.Waiting)) return; 
        
        // if (fsm is not PlayerFSM playerFsm) return;
        
        if (!UpdatedSprite(fsm))
        {
            _renderer.color = Color.clear;
        }
    }

    bool UpdatedSprite(FSM fsm)
    {
        string characterName = fsm.Name;
        var smear = fsm.StateMapConfig.SmearFrame;
        if (smear is null) return false;
        var frameSection = smear.Get(fsm, new FrameParam() { f = PredictedFrame, EntityRef = EntityRef });
        if (frameSection is null) return false;
        int frame = frameSection.GetCurrentItem(PredictedFrame, fsm) + 1;
        if (frame <= 0) return false;
        
        _renderer.color = Color.white;
        string fullPath = "Sprites/Characters/" + characterName + "/Smears/smears" + "" + frame;
        Sprite sprite = Resources.Load<Sprite>(fullPath);
        _renderer.sprite = sprite;
        var flip = !PredictedFrame.Get<PlayerDirection>(EntityRef).FacingRight;
        _renderer.flipX = flip;
        return true;
    }
    
}
