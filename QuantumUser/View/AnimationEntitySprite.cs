using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LayerMask = UnityEngine.LayerMask;

public class AnimationEntitySprite : QuantumEntityViewComponent
{
    private SpriteRenderer _renderer;
    
    
    public override void OnInitialize()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        AnimationEntities.AnimationEntityEnum animationEntityEnum = (AnimationEntities.AnimationEntityEnum)PredictedFrame.Get<AnimationEntityData>(EntityRef).type;
        var animationEntity = AnimationEntities.Get(animationEntityEnum);

        if (animationEntity.Layer is AELayer.Back) gameObject.layer = LayerMask.NameToLayer("AEBack");
        else if (animationEntity.Layer is AELayer.Front) gameObject.layer = LayerMask.NameToLayer("AEFront");
        else gameObject.layer = LayerMask.NameToLayer("AEMiddle");
    }

    public override void OnUpdateView()
    {
        if (!PredictedFrame.Has<AnimationEntityData>(EntityRef)) return;

        AnimationEntities.AnimationEntityEnum animationEntityEnum = (AnimationEntities.AnimationEntityEnum)PredictedFrame.Get<AnimationEntityData>(EntityRef).type;
        var animationEntity = AnimationEntities.Get(animationEntityEnum);
        string path = "Sprites/AnimationEntities/" + animationEntity.SpriteDirectory + "/Frames/" + animationEntity.SpriteDirectory;
        int frame = PredictedFrame.Get<AnimationEntityData>(EntityRef).spriteId + 1;
        Sprite sprite = Resources.Load<Sprite>(path + frame);
        
        _renderer.sprite = sprite;
        _renderer.flipX = PredictedFrame.Get<AnimationEntityData>(EntityRef).flip;
        var angle = PredictedFrame.Get<AnimationEntityData>(EntityRef).flip
            ? PredictedFrame.Get<AnimationEntityData>(EntityRef).angle.AsFloat
            : PredictedFrame.Get<AnimationEntityData>(EntityRef).angle.AsFloat * -1f;
        Vector3 rotationEuler = new Vector3(0, 0, angle);
        transform.rotation = Quaternion.Euler(rotationEuler);
    }
}
