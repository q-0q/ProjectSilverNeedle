using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationEntitySprite : QuantumEntityViewComponent
{
    private Image _image;
    
    
    public override void OnInitialize()
    {
        _image = GetComponent<Image>();
    }

    public override void OnUpdateView()
    {
        AnimationEntities.AnimationEntityEnum animationEntityEnum = (AnimationEntities.AnimationEntityEnum)PredictedFrame.Get<AnimationEntityData>(EntityRef).type;
        var animationEntity = AnimationEntities.Get(animationEntityEnum);
        string path = "Sprites/AnimationEntities/" + animationEntity.SpriteDirectory + "/Frames/" + animationEntity.SpriteDirectory;
        int frame = PredictedFrame.Get<AnimationEntityData>(EntityRef).spriteId + 1;
        Sprite sprite = Resources.Load<Sprite>(path + frame);
        _image.sprite = sprite;
    }
}
